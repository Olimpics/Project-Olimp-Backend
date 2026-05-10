using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IRatingRepository
{
    Task<List<Student>> GetStudentsForRatingAsync(int programId, int course, bool isAccelerated);
    Task<List<MainGrade>> GetMainGradesAsync(List<int> studentIds, int semester);
    Task<List<BindSelectiveDiscipline>> GetSelectiveGradesAsync(List<int> studentIds, int semester);
    Task<List<BindEventStudent>> GetEventPointsAsync(List<int> studentIds);
    Task<List<BindExtraActivity>> GetExtraActivityPointsAsync(List<int> studentIds);
    Task<Dictionary<int, int>> GetSgPointsMapAsync(List<int> studentIds);
    Task AddRatingsAsync(List<BindRating> ratings);
    Task AddCalculationTimeAsync(RatingCalculationTime calculationTime);
    Task<RatingCalculationTime?> GetCalculationTimeAsync(int specialityId, int course, int semester, int yearId, bool isAccelerated);
    Task<(List<BindRating> Items, int TotalCount)> GetPaginatedRatingsAsync(RatingListQueryDto query);
    Task SaveChangesAsync();
}

public class RatingRepository : IRatingRepository
{
    private readonly AppDbContext _context;

    public RatingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetStudentsForRatingAsync(int programId, int course, bool isAccelerated)
    {
        short isShortValue = (short)(isAccelerated ? 1 : 0);
        return await _context.Students
            .Include(s => s.Group)
                .ThenInclude(g => g.EducationalProgram)
            .Where(s => s.Group.EducationalProgramId == programId && s.Course == course && s.IsShort == isShortValue)
            .ToListAsync();
    }

    public async Task<List<MainGrade>> GetMainGradesAsync(List<int> studentIds, int semester)
    {
        return await _context.MainGrades
            .Include(mg => mg.MainDisciplines)
                .ThenInclude(md => md!.BindMainDiscipline)
            .Where(mg => mg.StudentId.HasValue && studentIds.Contains(mg.StudentId.Value) && 
                         mg.MainDisciplines != null && mg.MainDisciplines.Semestr == semester)
            .ToListAsync();
    }

    public async Task<List<BindSelectiveDiscipline>> GetSelectiveGradesAsync(List<int> studentIds, int semester)
    {
        return await _context.BindSelectiveDisciplines
            .Include(bsd => bsd.SelectiveDisciplines)
                .ThenInclude(sd => sd.SelectiveDetail)
            .Where(bsd => bsd.StudentId.HasValue && studentIds.Contains(bsd.StudentId.Value) && 
                         bsd.Semestr == semester &&
                         bsd.SelectiveDisciplines != null && 
                         bsd.SelectiveDisciplines.SelectiveDetail != null && 
                         bsd.SelectiveDisciplines.TypeOfControlId > 1)
            .ToListAsync();
    }

    public async Task<List<BindEventStudent>> GetEventPointsAsync(List<int> studentIds)
    {
        return await _context.BindEventStudents
            .Where(bes => bes.StudentId.HasValue && studentIds.Contains(bes.StudentId.Value))
            .ToListAsync();
    }

    public async Task<List<BindExtraActivity>> GetExtraActivityPointsAsync(List<int> studentIds)
    {
        return await _context.BindExtraActivities
            .Where(bea => bea.StudentId.HasValue && studentIds.Contains(bea.StudentId.Value))
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetSgPointsMapAsync(List<int> studentIds)
    {
        var members = await _context.MembersOfSgs
            .Include(m => m.BindsubdivisionRoleSg)
            .Where(m => m.StudentId.HasValue && studentIds.Contains(m.StudentId.Value))
            .ToListAsync();

        return members
            .Where(m => m.StudentId.HasValue && m.BindsubdivisionRoleSg != null && m.BindsubdivisionRoleSg.Points.HasValue)
            .GroupBy(m => m.StudentId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(m => m.BindsubdivisionRoleSg!.Points!.Value));
    }

    public async Task AddRatingsAsync(List<BindRating> ratings)
    {
        // Generating IDs if needed (ValueGeneratedNever)
        int nextId = 1;
        if (await _context.BindRatings.AnyAsync())
        {
            nextId = await _context.BindRatings.MaxAsync(r => r.IdBindRating) + 1;
        }

        foreach (var rating in ratings)
        {
            rating.IdBindRating = nextId++;
        }

        await _context.BindRatings.AddRangeAsync(ratings);
    }

    public async Task AddCalculationTimeAsync(RatingCalculationTime calculationTime)
    {
        int nextId = 1;
        if (await _context.RatingCalculationTimes.AnyAsync())
        {
            nextId = await _context.RatingCalculationTimes.MaxAsync(r => r.IdRatingCalculatioTime) + 1;
        }
        calculationTime.IdRatingCalculatioTime = nextId;

        await _context.RatingCalculationTimes.AddAsync(calculationTime);
    }

    public async Task<RatingCalculationTime?> GetCalculationTimeAsync(int specialityId, int course, int semester, int yearId, bool isAccelerated)
    {
        return await _context.RatingCalculationTimes
            .Where(rct => rct.SpecialityId == specialityId &&
                          rct.Course == course &&
                          rct.Semestr == semester &&
                          rct.YearId == yearId &&
                          rct.IsShorted != null && rct.IsShorted.Cast<bool>().FirstOrDefault() == isAccelerated)
            .OrderByDescending(rct => rct.Date)
            .FirstOrDefaultAsync();
    }

    public async Task<(List<BindRating> Items, int TotalCount)> GetPaginatedRatingsAsync(RatingListQueryDto query)
    {
        var bitSemester = new BitArray(new bool[] { query.Semester == 2 });
        
        var baseQuery = _context.BindRatings
            .Include(r => r.Student)
                .ThenInclude(s => s.Group)
            .Where(r => r.Year == query.CatalogYearId &&
                        r.Semestr == bitSemester &&
                        r.Student != null &&
                        r.Student.Group != null &&
                        r.Student.Group.EducationalProgram != null &&
                        r.Student.Group.EducationalProgram.SpecialityId == query.SpecialityId &&
                        r.Student.Course == query.Course &&
                        r.Student.IsShort == (short)(query.IsAccelerated ? 1 : 0));

        if (query.IsFundedOnly == true)
        {
            baseQuery = baseQuery.Where(r => r.Student!.IsFunded != null && r.Student.IsFunded.Cast<bool>().FirstOrDefault());
        }

        if (query.NoRetakesOnly == true)
        {
            baseQuery = baseQuery.Where(r => r.IsRedo != null && !r.IsRedo.Cast<bool>().FirstOrDefault());
        }

        int totalCount = await baseQuery.CountAsync();
        var items = await baseQuery
            .OrderByDescending(r => r.FinalScore)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
