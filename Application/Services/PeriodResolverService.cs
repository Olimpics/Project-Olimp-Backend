using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

/// <summary>
/// Single source of truth for determining which discipline-choice period applies
/// to a given student. Previously this logic was embedded in StudentChoiceCacheService.
/// </summary>
public class PeriodResolverService : IPeriodResolverService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PeriodResolverService> _logger;

    public PeriodResolverService(AppDbContext context, ILogger<PeriodResolverService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PeriodResolutionResult> ResolveActivePeriodAsync(Guid studentId)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.Group)
                .ThenInclude(g => g.EducationalProgram)
                    .ThenInclude(ep => ep.Speciality)
            .FirstOrDefaultAsync(s => s.IdStudent == studentId);

        if (student?.Group?.EducationalProgram == null || student.Group.AdmissionYear == null)
            return PeriodResolutionResult.None();

        var ep = student.Group.EducationalProgram;
        int admissionYear = student.Group.AdmissionYear.Value.Year;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var candidates = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Include(p => p.CatalogYear)
            .Where(p => p.IsConfirm
                        && !p.IsClose
                        && p.DepartmentId == ep.Speciality.DepartmentId
                        && p.DegreeLevelId == ep.DegreeId
                        && p.IsShort == ep.IsAccelerated)
            .ToListAsync();

        var matching = candidates.Where(p =>
        {
            int course = p.CatalogYear.YearStart - admissionYear + 1;
            bool courseOk = p.PeriodCourse == 0 || p.PeriodCourse == course;
            bool specialityOk = p.SpecialityId == null || p.SpecialityId == ep.SpecialityId;
            bool dateOk = today >= p.StartDate && today <= p.EndDate;
            return courseOk && specialityOk && dateOk;
        }).ToList();

        if (matching.Count == 0)
            return PeriodResolutionResult.None();

        if (matching.Count == 1)
            return PeriodResolutionResult.Found(matching[0]);

        // Resolve by specificity: a speciality-/course-targeted period wins over a general one.
        int Specificity(DisciplineChoicePeriod p) =>
            (p.SpecialityId != null ? 1 : 0) + (p.PeriodCourse != 0 ? 1 : 0);

        int maxScore = matching.Max(Specificity);
        var best = matching.Where(p => Specificity(p) == maxScore).ToList();

        if (best.Count == 1)
            return PeriodResolutionResult.Found(best[0]);

        var ids = best.Select(p => p.IdDisciplineChoicePeriod).ToList();
        _logger.LogError(
            "Period configuration conflict for student {StudentId}: {Count} equally-suitable periods {Ids}",
            studentId, best.Count, string.Join(", ", ids));
        return PeriodResolutionResult.Conflict(ids);
    }

    public async Task<List<Student>> GetStudentsForPeriodAsync(DisciplineChoicePeriod period)
    {
        if (period.CatalogYear == null)
            await _context.Entry(period).Reference(p => p.CatalogYear).LoadAsync();

        var students = await _context.Students
            .AsNoTracking()
            .Include(s => s.Group)
                .ThenInclude(g => g.EducationalProgram)
                    .ThenInclude(ep => ep.Speciality)
            .Where(s => s.Avail
                        && s.Group.EducationalProgram.Speciality.DepartmentId == period.DepartmentId
                        && s.Group.EducationalProgram.DegreeId == period.DegreeLevelId
                        && s.Group.EducationalProgram.IsAccelerated == period.IsShort)
            .ToListAsync();

        if (period.SpecialityId.HasValue)
            students = students.Where(s => s.Group.EducationalProgram.SpecialityId == period.SpecialityId.Value).ToList();

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
}
