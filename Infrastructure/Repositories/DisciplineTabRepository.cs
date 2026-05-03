using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDisciplineTabRepository
{
    Task<List<SelectiveDiscipline>> GetDisciplinesForAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto);
    Task<List<SelectiveDiscipline>> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto);
    Task<bool> IsChoicePeriodActiveAsync(int facultyId, DateTime now);
    Task<SelectiveDiscipline?> GetDisciplineByIdAsNoTrackingAsync(int id);
    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsDtoAsync(int id);
    Task<SelectiveDiscipline?> GetDisciplineWithDetailEntityAsync(int id);
    Task<bool> DepartmentExistsAsync(int departmentId);
    Task SelectiveDisciplineAsync(SelectiveDiscipline discipline);
    Task AddBindAsync(BindSelectiveDiscipline bind);
    Task SaveChangesAsync();
}

public class DisciplineTabRepository : IDisciplineTabRepository
{
    private readonly AppDbContext _context;

    public DisciplineTabRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SelectiveDiscipline>> GetDisciplinesForAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto)
    {
        var query = _context.SelectiveDisciplines
            .Include(d => d.DegreeLevel)
            .Include(d => d.Faculty)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => d.FacultyId.HasValue && queryDto.Faculties.Contains(d.FacultyId.Value));

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
    public async Task<List<SelectiveDiscipline>> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto)
    {
        var isEven = queryDto.IsEvenSemester ? (sbyte)0 : (sbyte)1;

        return await _context.SelectiveDisciplines
            .Where(d => d.IsEven == isEven)
            .ToListAsync();
    }
    public async Task<bool> IsChoicePeriodActiveAsync(int facultyId, DateTime now)
    {
        var rows = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.FacultyId == facultyId && p.StartDate != null && p.EndDate != null)
            .Select(p => new { p.StartDate, p.EndDate })
            .ToListAsync();

        foreach (var p in rows)
        {
            if (!System.DateTime.TryParse(p.StartDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var start))
                continue;
            if (!System.DateTime.TryParse(p.EndDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var end))
                continue;
            if (start <= now && end >= now)
                return true;
        }

        return false;
    }
    public async Task<SelectiveDiscipline?> GetDisciplineByIdAsNoTrackingAsync(int id) =>
        await _context.SelectiveDisciplines.AsNoTracking().FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == id);

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsDtoAsync(int id)
    {
        // Проекція без Include
        return await _context.SelectiveDisciplines
            .Where(d => d.IdSelectiveDisciplines == id)
            .Select(d => new FullDisciplineWithDetailsDto
            {
                IdSelectiveDisciplines = d.IdSelectiveDisciplines,
                NameSelectiveDisciplines = d.NameSelectiveDisciplines ?? "",
                CodeSelectiveDisciplines = d.CodeSelectiveDisciplines ?? "",
                FacultyAbbreviation = d.Faculty != null ? d.Faculty.Abbreviation : null,
                MinCountPeople = d.MinCountPeople,
                MaxCountPeople = d.MaxCountPeople,
                MinCourse = d.MinCourse,
                MaxCourse = d.MaxCourse,
                IsEven = d.IsEven.HasValue ? (sbyte?)d.IsEven.Value : null,
                DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegree : "",
                DepartmentName = d.SelectiveDetail != null && d.SelectiveDetail.Department != null ? d.SelectiveDetail.Department.NameDepartment : "",
                Teacher = d.SelectiveDetail != null ? d.SelectiveDetail.Teachers : null,
                Recomend = d.SelectiveDetail != null ? d.SelectiveDetail.Recomend : null,
                Prerequisites = d.SelectiveDetail != null ? d.SelectiveDetail.Prerequisites : null,
                Language = d.SelectiveDetail != null ? d.SelectiveDetail.Language : null,
                Provision = d.SelectiveDetail != null ? d.SelectiveDetail.Provision : null,
                WhyInterestingDetermination = d.SelectiveDetail != null ? d.SelectiveDetail.WhyInterestingDetermination : null,
                ResultEducation = d.SelectiveDetail != null ? d.SelectiveDetail.ResultEducation : null,
                UsingIrl = d.SelectiveDetail != null ? d.SelectiveDetail.UsingIrl : null,
                DisciplineTopics = d.SelectiveDetail != null ? d.SelectiveDetail.DisciplineTopics : null,
                TypesOfTraining = d.SelectiveDetail != null ? d.SelectiveDetail.TypesOfTraining : "",
                TypeOfControl = d.SelectiveDetail != null ? d.SelectiveDetail.TypeOfControll != null ? d.SelectiveDetail.TypeOfControll.Type.ToString() : "" : ""
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SelectiveDiscipline?> GetDisciplineWithDetailEntityAsync(int id) =>
        await _context.SelectiveDisciplines.Include(d => d.SelectiveDetail).FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == id);

    public async Task<bool> DepartmentExistsAsync(int departmentId) =>
        await _context.Departments.AnyAsync(d => d.IdDepartment == departmentId);

    public async Task SelectiveDisciplineAsync(SelectiveDiscipline discipline) =>
        await _context.SelectiveDisciplines.AddAsync(discipline);

    public async Task AddBindAsync(BindSelectiveDiscipline bind) =>
        await _context.BindSelectiveDisciplines.AddAsync(bind);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}