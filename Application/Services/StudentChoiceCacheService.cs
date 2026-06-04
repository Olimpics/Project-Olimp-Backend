using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;
using StackExchange.Redis;

namespace OlimpBack.Application.Services;

public class StudentChoiceCacheService : IStudentChoiceCacheService
{
    private readonly IDatabase _database;
    private readonly AppDbContext _context;
    private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private const string CacheKeyPrefix = "student_choice:";

    public StudentChoiceCacheService(IConnectionMultiplexer redis, AppDbContext context)
    {
        _database = redis.GetDatabase();
        _context = context;
    }

    public async Task<int[]> GetLimitsAsync(Guid studentId)
    {
        string cacheKey = $"{CacheKeyPrefix}{studentId}";
        var cached = await _database.StringGetAsync(cacheKey);

        if (cached.HasValue)
        {
            return JsonSerializer.Deserialize<int[]>(cached!) ?? new[] { 0, 0 };
        }

        // Cache Stampede protection
        await _lock.WaitAsync();
        try
        {
            // Double-check cache
            cached = await _database.StringGetAsync(cacheKey);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<int[]>(cached!) ?? new[] { 0, 0 };
            }

            var limits = await CalculateRemainingLimitsAsync(studentId);
            await _database.StringSetAsync(cacheKey, JsonSerializer.Serialize(limits), TimeSpan.FromHours(24));
            return limits;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateLimitAsync(Guid studentId, bool isSpring, int delta)
    {
        string cacheKey = $"{CacheKeyPrefix}{studentId}";
        
        // We use a lock to ensure atomic update of the array in Redis
        // Or we could use a Lua script for better performance in a distributed environment
        await _lock.WaitAsync();
        try
        {
            var limits = await GetLimitsAsync(studentId);
            int index = isSpring ? 1 : 0;
            limits[index] += delta;
            
            // Ensure not negative
            if (limits[index] < 0) limits[index] = 0;

            await _database.StringSetAsync(cacheKey, JsonSerializer.Serialize(limits), TimeSpan.FromHours(24));
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task ClearCacheAsync(Guid studentId)
    {
        string cacheKey = $"{CacheKeyPrefix}{studentId}";
        await _database.KeyDeleteAsync(cacheKey);
    }

    public async Task ClearCacheForStudentsInPeriodAsync(DisciplineChoicePeriod period)
    {
        var students = await GetStudentsForPeriodInternalAsync(period);
        foreach (var student in students)
        {
            await ClearCacheAsync(student.IdStudent);
        }
    }

    public async Task InitializeCacheAsync(Guid? studentId, DisciplineChoicePeriod? period)
    {
        if (studentId.HasValue)
        {
            var limits = await CalculateRemainingLimitsAsync(studentId.Value);
            string cacheKey = $"{CacheKeyPrefix}{studentId.Value}";
            await _database.StringSetAsync(cacheKey, JsonSerializer.Serialize(limits), TimeSpan.FromHours(24));
            return;
        }

        if (period == null) return;

        var students = await GetStudentsForPeriodInternalAsync(period);
        if (!students.Any()) return;

        var studentIds = students.Select(s => s.IdStudent).ToList();

        // Batch load choices to calculate remaining limits efficiently
        var allChoices = await _context.BindSelectiveDisciplines
            .Where(b => studentIds.Contains(b.StudentId) && b.YearId == period.CatalogYearId)
            .ToListAsync();

        var choicesByStudent = allChoices.GroupBy(b => b.StudentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var tasks = students.Select(async student =>
        {
            int admissionYear = student.Group.AdmissionYear!.Value.Year;
            int course = period.CatalogYear!.YearStart - admissionYear + 1;
            var ep = student.Group.EducationalProgram;

            if (ep.SelectiveDisciplineBySemestr == null || ep.SelectiveDisciplineBySemestr.Count < course * 2)
                return;

            int totalFall = ep.SelectiveDisciplineBySemestr[(course - 1) * 2];
            int totalSpring = ep.SelectiveDisciplineBySemestr[(course - 1) * 2 + 1];

            if (!period.IsForBothSemester) totalFall = 0;

            choicesByStudent.TryGetValue(student.IdStudent, out var chosen);
            int chosenFall = chosen?.Count(b => b.Semestr % 2 != 0) ?? 0;
            int chosenSpring = chosen?.Count(b => b.Semestr % 2 == 0) ?? 0;

            int[] limits = new[] { Math.Max(0, totalFall - chosenFall), Math.Max(0, totalSpring - chosenSpring) };
            
            string cacheKey = $"{CacheKeyPrefix}{student.IdStudent}";
            await _database.StringSetAsync(cacheKey, JsonSerializer.Serialize(limits), TimeSpan.FromHours(24));
        });

        await Task.WhenAll(tasks);
    }

    private async Task<List<Student>> GetStudentsForPeriodInternalAsync(DisciplineChoicePeriod period)
    {
        // Ensure CatalogYear is loaded
        if (period.CatalogYear == null)
        {
            await _context.Entry(period).Reference(p => p.CatalogYear).LoadAsync();
        }

        var students = await _context.Students
            .Include(s => s.Group)
                .ThenInclude(g => g.EducationalProgram)
                    .ThenInclude(ep => ep.Speciality)
            .Where(s => s.Avail && 
                        s.Group.EducationalProgram.Speciality.DepartmentId == period.DepartmentId &&
                        s.Group.EducationalProgram.DegreeId == period.DegreeLevelId &&
                        s.Group.EducationalProgram.IsAccelerated == period.IsShort)
            .ToListAsync();

        if (period.SpecialityId.HasValue)
        {
            students = students.Where(s => s.Group.EducationalProgram.SpecialityId == period.SpecialityId.Value).ToList();
        }

        var result = new List<Student>();
        foreach (var student in students)
        {
            if (student.Group.AdmissionYear == null) continue;
            
            int admissionYear = student.Group.AdmissionYear.Value.Year;
            int course = period.CatalogYear.YearStart - admissionYear + 1;

            if (period.PeriodCourse > 0 && period.PeriodCourse != course) continue;
            
            result.Add(student);
        }
        return result;
    }

    private async Task<int[]> CalculateRemainingLimitsAsync(Guid studentId)
    {
        var student = await _context.Students
            .Include(s => s.Group)
                .ThenInclude(g => g.EducationalProgram)
                    .ThenInclude(ep => ep.Speciality)
            .FirstOrDefaultAsync(s => s.IdStudent == studentId);

        if (student == null || student.Group.AdmissionYear == null)
            return new[] { 0, 0 };

        int admissionYear = student.Group.AdmissionYear.Value.Year;

        // Find active period for this student
        var periods = await _context.DisciplineChoicePeriods
            .Include(p => p.CatalogYear)
            .Where(p => p.IsConfirm && !p.IsClose && p.DepartmentId == student.Group.EducationalProgram.Speciality.DepartmentId)
            .ToListAsync();

        var period = periods.FirstOrDefault(p => 
            (p.PeriodCourse == 0 || p.PeriodCourse == (p.CatalogYear.YearStart - admissionYear + 1)) &&
            p.DegreeLevelId == student.Group.EducationalProgram.DegreeId &&
            p.IsShort == student.Group.EducationalProgram.IsAccelerated &&
            (p.SpecialityId == null || p.SpecialityId == student.Group.EducationalProgram.SpecialityId)
        );

        if (period == null)
            return new[] { 0, 0 };

        int course = period.CatalogYear.YearStart - admissionYear + 1;
        var ep = student.Group.EducationalProgram;

        if (ep.SelectiveDisciplineBySemestr == null || ep.SelectiveDisciplineBySemestr.Count < course * 2)
            return new[] { 0, 0 };

        int totalFall = ep.SelectiveDisciplineBySemestr[(course - 1) * 2];
        int totalSpring = ep.SelectiveDisciplineBySemestr[(course - 1) * 2 + 1];

        if (!period.IsForBothSemester)
        {
            // "take either both semesters (even and odd) or only the second"
            totalFall = 0;
        }

        // Count already chosen
        var chosen = await _context.BindSelectiveDisciplines
            .Where(b => b.StudentId == studentId && b.YearId == period.CatalogYearId)
            .ToListAsync();

        int chosenFall = chosen.Count(b => b.Semestr % 2 != 0);
        int chosenSpring = chosen.Count(b => b.Semestr % 2 == 0);

        int remainingFall = Math.Max(0, totalFall - chosenFall);
        int remainingSpring = Math.Max(0, totalSpring - chosenSpring);

        return new[] { remainingFall, remainingSpring };
    }
}
