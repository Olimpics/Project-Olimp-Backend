using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IRatingRepository
{
    Task<List<Student>> GetStudentsForRatingAsync(Guid programId, int course, bool isAccelerated);
    Task<List<MainGrade>> GetMainGradesAsync(List<Guid> studentIds, int semester);
    Task<List<BindSelectiveDiscipline>> GetSelectiveGradesAsync(List<Guid> studentIds, int semester);
    Task<List<BindEventStudent>> GetEventPointsAsync(List<Guid> studentIds);
    Task<List<BindExtraActivity>> GetExtraActivityPointsAsync(List<Guid> studentIds);
    Task<Dictionary<Guid, int>> GetSgPointsMapAsync(List<Guid> studentIds);
    Task AddRatingsAsync(List<BindRating> ratings);
    Task AddCalculationTimeAsync(RatingCalculationTime calculationTime);
    Task<RatingCalculationTime?> GetCalculationTimeAsync(Guid specialityId, int course, int semester, Guid yearId, bool isAccelerated);
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

    public async Task<List<Student>> GetStudentsForRatingAsync(Guid programId, int course, bool isAccelerated)
    {
        return await _context.Students
            .Include(s => s.Group)
                .ThenInclude(g => g.EducationalProgram)
            .Where(s => s.Group != null && s.Group.EducationalProgramId == programId && s.Course == course && s.IsShort == isAccelerated)
            .ToListAsync();
    }

    public async Task<List<MainGrade>> GetMainGradesAsync(List<Guid> studentIds, int semester)
    {
        return await _context.MainGrades
            .Include(mg => mg.MainDisciplines)
                .ThenInclude(md => md!.BindMainDisciplines)
            .Where(mg => studentIds.Contains(mg.StudentId) && 
                         mg.MainDisciplines != null && mg.MainDisciplines.Semestr == semester)
            .ToListAsync();
    }

    public async Task<List<BindSelectiveDiscipline>> GetSelectiveGradesAsync(List<Guid> studentIds, int semester)
    {
        return await _context.BindSelectiveDisciplines
            .Include(bsd => bsd.SelectiveDiscipline)
                .ThenInclude(sd => sd.SelectiveDetail)
            .Where(bsd => studentIds.Contains(bsd.StudentId) && 
                         bsd.Semestr == semester &&
                         bsd.SelectiveDiscipline != null && 
                         bsd.SelectiveDiscipline.SelectiveDetail != null && 
                         bsd.SelectiveDiscipline.TypeOfControlId != Guid.Empty)
            .ToListAsync();
    }

    public async Task<List<BindEventStudent>> GetEventPointsAsync(List<Guid> studentIds)
    {
        return await _context.BindEventStudents
            .Where(bes => studentIds.Contains(bes.StudentId))
            .ToListAsync();
    }

    public async Task<List<BindExtraActivity>> GetExtraActivityPointsAsync(List<Guid> studentIds)
    {
        return await _context.BindExtraActivities
            .Where(bea => studentIds.Contains(bea.StudentId))
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetSgPointsMapAsync(List<Guid> studentIds)
    {
        var members = await _context.MembersOfSgs
            .Include(m => m.BindsubdivisionRoleSg)
            .Where(m => m.StudentId != null && studentIds.Contains(m.StudentId))
            .ToListAsync();

        return members
            .Where(m => m.StudentId != null && m.BindsubdivisionRoleSg != null && m.BindsubdivisionRoleSg.Points != null)
            .GroupBy(m => m.StudentId)
            .ToDictionary(g => g.Key, g => g.Sum(m => m.BindsubdivisionRoleSg!.Points!.Value));
    }

    public async Task AddRatingsAsync(List<BindRating> ratings)
    {
        foreach (var rating in ratings)
        {
            if (rating.IdBindRating == Guid.Empty)
                rating.IdBindRating = Guid.NewGuid();
        }

        await _context.BindRatings.AddRangeAsync(ratings);
    }

    public async Task AddCalculationTimeAsync(RatingCalculationTime calculationTime)
    {
        if (calculationTime.IdRatingCalculationTime == Guid.Empty)
            calculationTime.IdRatingCalculationTime = Guid.NewGuid();

        await _context.RatingCalculationTimes.AddAsync(calculationTime);
    }

    public async Task<RatingCalculationTime?> GetCalculationTimeAsync(Guid specialityId, int course, int semester, Guid yearId, bool isAccelerated)
    {
        bool isEven = (semester == 2);
        return await _context.RatingCalculationTimes
            .Where(rct => rct.SpecialityId == specialityId &&
                          rct.Course == course &&
                          rct.IsEven == isEven &&
                          rct.YearId == yearId &&
                          rct.IsShorted == isAccelerated)
            .OrderByDescending(rct => rct.Date)
            .FirstOrDefaultAsync();
    }

    public async Task<(List<BindRating> Items, int TotalCount)> GetPaginatedRatingsAsync(RatingListQueryDto query)
    {
        bool isEven = (query.Semester == 2);
        var baseQuery = _context.BindRatings
            .Include(r => r.Student)
                .ThenInclude(s => s.Group)
            .Where(r => r.IsEven == isEven &&
                        r.Student != null &&
                        r.Student.Group != null &&
                        r.Student.Group.EducationalProgram != null &&
                        r.Student.Group.EducationalProgram.SpecialityId == query.SpecialityId &&
                        r.Student.Course == query.Course &&
                        r.Student.IsShort == query.IsAccelerated);


        if (query.IsFundedOnly == true)
        {
            baseQuery = baseQuery.Where(r => r.Student!.IsFunded == true);
        }

        if (query.NoRetakesOnly == true)
        {
            baseQuery = baseQuery.Where(r => r.IsRedo == false);
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
