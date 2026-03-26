using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDisciplineTabRepository
{
    Task<List<AddDiscipline>> GetDisciplinesForAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto);
    Task<List<AddDiscipline>> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto);
    Task<bool> IsChoicePeriodActiveAsync(int facultyId, DateTime now);
    Task<AddDiscipline?> GetDisciplineByIdAsNoTrackingAsync(int id);
    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsDtoAsync(int id);
    Task<AddDiscipline?> GetDisciplineWithDetailEntityAsync(int id);
    Task<bool> DepartmentExistsAsync(int departmentId);
    Task AddDisciplineAsync(AddDiscipline discipline);
    Task AddBindAsync(BindAddDiscipline bind);
    Task SaveChangesAsync();
}

public class DisciplineTabRepository : IDisciplineTabRepository
{
    private readonly AppDbContext _context;

    public DisciplineTabRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AddDiscipline>> GetDisciplinesForAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto)
    {
        var query = _context.AddDisciplines
            .Include(d => d.DegreeLevel)
            .Include(d => d.Faculty)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => queryDto.Faculties.Contains(d.FacultyId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(d =>
                (!d.MinCourse.HasValue || queryDto.Courses.Contains(d.MinCourse.Value)) &&
                (!d.MaxCourse.HasValue || queryDto.Courses.Contains(d.MaxCourse.Value)));

        if (queryDto.IsEvenSemester.HasValue)
        {
            var semesterValue = queryDto.IsEvenSemester.Value ? (sbyte)0 : (sbyte)1;
            query = query.Where(d => d.IsEven == semesterValue);
        }

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));

        return await query.ToListAsync();
    }
    public async Task<List<AddDiscipline>> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto)
    {
        var isEven = queryDto.IsEvenSemester ? (sbyte)0 : (sbyte)1;

        return await _context.AddDisciplines
            .Where(d => d.IsEven == isEven)
            .ToListAsync();
    }
    public async Task<bool> IsChoicePeriodActiveAsync(int facultyId, DateTime now)
    {
        return await _context.DisciplineChoicePeriods
            .AnyAsync(p =>
                p.FacultyId == facultyId &&
                p.StartDate <= now &&
                p.EndDate >= now);
    }
    public async Task<AddDiscipline?> GetDisciplineByIdAsNoTrackingAsync(int id) =>
        await _context.AddDisciplines.AsNoTracking().FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsDtoAsync(int id)
    {
        // Проекція без Include
        return await _context.AddDisciplines
            .Where(d => d.IdAddDisciplines == id)
            .Select(d => new FullDisciplineWithDetailsDto
            {
                IdAddDisciplines = d.IdAddDisciplines,
                NameAddDisciplines = d.NameAddDisciplines,
                CodeAddDisciplines = d.CodeAddDisciplines,
                FacultyAbbreviation = d.Faculty != null ? d.Faculty.Abbreviation : null,
                MinCountPeople = d.MinCountPeople,
                MaxCountPeople = d.MaxCountPeople,
                MinCourse = d.MinCourse,
                MaxCourse = d.MaxCourse,
                IsEven = d.IsEven,
                DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegreec : "",
                DepartmentName = d.AddDetail != null && d.AddDetail.Department != null ? d.AddDetail.Department.NameDepartment : "",
                Teacher = d.AddDetail != null ? d.AddDetail.Teachers : null,
                Recomend = d.AddDetail != null ? d.AddDetail.Recomend : null,
                Prerequisites = d.AddDetail != null ? d.AddDetail.Prerequisites : null,
                Language = d.AddDetail != null ? d.AddDetail.Language : null,
                Provision = d.AddDetail != null ? d.AddDetail.Provision : null,
                WhyInterestingDetermination = d.AddDetail != null ? d.AddDetail.WhyInterestingDetermination : null,
                ResultEducation = d.AddDetail != null ? d.AddDetail.ResultEducation : null,
                UsingIrl = d.AddDetail != null ? d.AddDetail.UsingIrl : null,
                DisciplineTopics = d.AddDetail != null ? d.AddDetail.DisciplineTopics : null,
                TypesOfTraining = d.AddDetail != null ? d.AddDetail.TypesOfTraining : "",
                TypeOfControll = d.AddDetail != null ? d.AddDetail.TypeOfControll : ""
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AddDiscipline?> GetDisciplineWithDetailEntityAsync(int id) =>
        await _context.AddDisciplines.Include(d => d.AddDetail).FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

    public async Task<bool> DepartmentExistsAsync(int departmentId) =>
        await _context.Departments.AnyAsync(d => d.IdDepartment == departmentId);

    public async Task AddDisciplineAsync(AddDiscipline discipline) =>
        await _context.AddDisciplines.AddAsync(discipline);

    public async Task AddBindAsync(BindAddDiscipline bind) =>
        await _context.BindAddDisciplines.AddAsync(bind);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}