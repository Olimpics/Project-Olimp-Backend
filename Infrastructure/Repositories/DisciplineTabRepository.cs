using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDisciplineTabRepository
{
    Task<List<SelectiveDiscipline>> GetDisciplinesForAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto);
    Task<List<SelectiveDiscipline>> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto);
    Task<bool> IsChoicePeriodActiveAsync(Guid facultyId, DateTime now);
    Task<SelectiveDiscipline?> GetDisciplineByIdAsNoTrackingAsync(Guid id);
    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsDtoAsync(Guid id);
    Task<SelectiveDiscipline?> GetDisciplineWithDetailEntityAsync(Guid id);
    Task<bool> DepartmentExistsAsync(Guid departmentId);
    Task SelectiveDisciplineAsync(SelectiveDiscipline discipline);
    Task AddBindAsync(BindSelectiveDiscipline bind);
    Task<List<SimilarDisciplineGroupDto>> GetSimilarDisciplinesAsync(Guid disciplineId);
    Task<bool> RemoveDisciplineFromSimilarityGroupAsync(Guid disciplineId, Guid groupId);
    Task<StudentOwnSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(StudentOwnSelectiveDisciplinesQueryDto query);
    Task<StudentEducationalProgramWithGradesDto?> GetStudentEducationalProgramWithGradesAsync(Guid studentId);
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
            .Include(d => d.Department.Faculty)
            .Include(d => d.SelectiveDetail)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                (d.SelectiveDetail != null && EF.Functions.Like(d.SelectiveDetail.Teachers.ToLower(), $"%{lowerSearch}%")));
        }

        if (queryDto.CatalogId.HasValue && queryDto.CatalogId != Guid.Empty)
            query = query.Where(d => d.CatalogId == queryDto.CatalogId.Value);

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => d.Department.FacultyId != null && queryDto.Faculties.Contains(d.Department.FacultyId));

        if (queryDto.IsEvenSemester.HasValue)
        {
            query = query.Where(d => d.IsEven == null || d.IsEven == queryDto.IsEvenSemester.Value);
        }

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId != Guid.Empty && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId));

        if (queryDto.TypeOfControlIds != null && queryDto.TypeOfControlIds.Any())
            query = query.Where(d =>d.TypeOfControlId != Guid.Empty && queryDto.TypeOfControlIds.Contains(d.TypeOfControlId));

        if (queryDto.ApprovalStatusIds != null && queryDto.ApprovalStatusIds.Any())
            query = query.Where(d => d.ApprovalStatusId != Guid.Empty && queryDto.ApprovalStatusIds.Contains(d.ApprovalStatusId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(d =>
                (!d.Courses.Any() || queryDto.Courses.Any(c => d.Courses.Contains(c))));

        return await query.ToListAsync();
    }
    public async Task<List<SelectiveDiscipline>> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto)
    {
        return await _context.SelectiveDisciplines
            .Where(d => d.IsEven == null || d.IsEven == queryDto.IsEvenSemester)
            .ToListAsync();
    }
    public async Task<bool> IsChoicePeriodActiveAsync(Guid facultyId, DateTime now)
    {
        var dateNow = DateOnly.FromDateTime(now);
        return await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .AnyAsync(p => p.Department != null && p.Department.FacultyId == facultyId && p.StartDate != null && p.EndDate != null && p.StartDate <= dateNow && p.EndDate >= dateNow);
    }
    public async Task<SelectiveDiscipline?> GetDisciplineByIdAsNoTrackingAsync(Guid id) =>
        await _context.SelectiveDisciplines.AsNoTracking().FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == id);

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsDtoAsync(Guid id)
    {
        var d = await _context.SelectiveDisciplines
            .Include(d => d.Department.Faculty)
            .Include(d => d.DegreeLevel)
            .Include(d => d.SelectiveDetail)
            .Include(d => d.TypeOfControl)
            .Include(d => d.ApprovalStatus)
            .Include(d => d.Catalog)
            .FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == id);

        if (d == null) return null;

        return new FullDisciplineWithDetailsDto
        {
            IdSelectiveDisciplines = d.IdSelectiveDisciplines,
            NameSelectiveDisciplines = d.NameSelectiveDisciplines ?? "",
            CodeSelectiveDisciplines = d.CodeSelectiveDisciplines ?? "",
            FacultyAbbreviation = d.Department.Faculty != null ? d.Department.Faculty.Abbreviation : null,
            MinCountPeople = d.MinCountPeople,
            MaxCountPeople = d.MaxCountPeople,
            Courses = d.Courses != null ? d.Courses.ToList() : new List<int>(),
            IsEven = d.IsEven,
            DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegree : "",
            NameSelectiveDisciplinesEng = d.SelectiveDetail != null ? d.SelectiveDetail.NameSelectiveDisciplinesEng : null,
            DepartmentName = d.Department != null ? d.Department.NameDepartment : "",
            Teacher = d.SelectiveDetail != null ? d.SelectiveDetail.Teachers : null,
            Recommended = !string.IsNullOrEmpty(d.SelectiveDetail?.Recommended) 
                ? System.Text.Json.JsonSerializer.Deserialize<FullDisciplineWithDetailsDto.RecommendedDto>(d.SelectiveDetail.Recommended, (System.Text.Json.JsonSerializerOptions?)null) 
                : null,
            Prerequisites = d.SelectiveDetail != null ? d.SelectiveDetail.Prerequisites : null,
            Language = d.SelectiveDetail != null ? d.SelectiveDetail.Language : null,
            Provision = d.SelectiveDetail != null ? d.SelectiveDetail.Provision : null,
            WhyInterestingDetermination = d.SelectiveDetail != null ? d.SelectiveDetail.WhyInterestingDetermination : null,
            ResultEducation = d.SelectiveDetail != null ? d.SelectiveDetail.ResultEducation : null,
            UsingIrl = d.SelectiveDetail != null ? d.SelectiveDetail.UsingIrl : null,
            DisciplineTopics = d.SelectiveDetail != null ? d.SelectiveDetail.DisciplineTopics : null,
            TypesOfTraining = d.SelectiveDetail != null ? d.SelectiveDetail.TypesOfTraining : "",
            TypeOfControl = d.TypeOfControl != null ? d.TypeOfControl.Type : "",
            CatalogId = d.CatalogId,
            ApprovalStatusId = d.ApprovalStatusId,
            TypeOfControlId = d.TypeOfControlId,
            Feedback = d.Feedback,
            IsForseChange = d.IsForseChange,
            NameDock = d.NameDock,
            Keys = d.Keys,
            ApprovalStatus = d.ApprovalStatus != null ? d.ApprovalStatus.AppovalStatus : null,
            NeedFix = d.NeedFix,
            YearStart = d.Catalog != null ? d.Catalog.YearStart : 0,
            YearEnd = d.Catalog != null ? d.Catalog.YearEnd : 0
        };
    }

    public async Task<SelectiveDiscipline?> GetDisciplineWithDetailEntityAsync(Guid id) =>
        await _context.SelectiveDisciplines.Include(d => d.SelectiveDetail).FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == id);

    public async Task<bool> DepartmentExistsAsync(Guid departmentId) =>
        await _context.Departments.AnyAsync(d => d.IdDepartment == departmentId);

    public async Task SelectiveDisciplineAsync(SelectiveDiscipline discipline) =>
        await _context.SelectiveDisciplines.AddAsync(discipline);

    public async Task AddBindAsync(BindSelectiveDiscipline bind) =>
        await _context.BindSelectiveDisciplines.AddAsync(bind);

    public async Task<List<SimilarDisciplineGroupDto>> GetSimilarDisciplinesAsync(Guid disciplineId)
    {
        var groupIds = await _context.BindSimilarSelectiveInGroups
            .Where(b => b.SelectiveId == disciplineId)
            .Select(b => b.GroupId)
            .ToListAsync();

        if (!groupIds.Any()) return new List<SimilarDisciplineGroupDto>();

        return await _context.GroupSimilarSelectives
            .Where(g => groupIds.Contains(g.IdGroup))
            .Include(g => g.Centrall)
            .Include(g => g.BindSimilarSelectiveInGroups)
                .ThenInclude(b => b.Selective)
                    .ThenInclude(s => s.Catalog)
            .Select(g => new SimilarDisciplineGroupDto
            {
                GroupName = g.Centrall.NameSelectiveDisciplines,
                Disciplines = g.BindSimilarSelectiveInGroups.Select(b => new SimilarDisciplineDto
                {
                    Id = b.Selective.IdSelectiveDisciplines,
                    Name = b.Selective.NameSelectiveDisciplines,
                    YearStart = b.Selective.Catalog != null ? b.Selective.Catalog.YearStart : 0,
                    YearEnd = b.Selective.Catalog != null ? b.Selective.Catalog.YearEnd : 0
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<bool> RemoveDisciplineFromSimilarityGroupAsync(Guid disciplineId, Guid groupId)
    {
        var bind = await _context.BindSimilarSelectiveInGroups
            .FirstOrDefaultAsync(b => b.SelectiveId == disciplineId && b.GroupId == groupId);

        if (bind == null) return false;

        _context.BindSimilarSelectiveInGroups.Remove(bind);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<StudentOwnSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(
        StudentOwnSelectiveDisciplinesQueryDto query)
    {
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdStudent == query.StudentId);

        if (student == null) return null;

        var bindsQuery = _context.BindSelectiveDisciplines
            .AsNoTracking()
            .Where(b => b.StudentId == query.StudentId)
            .Include(b => b.SelectiveDiscipline)
                .ThenInclude(d => d.Catalog)
            .Include(b => b.SelectiveDiscipline)
                .ThenInclude(d => d.Department)
                    .ThenInclude(dept => dept.Faculty)
            .Include(b => b.Year)
            .AsQueryable();

        // Filter by CatalogYearsSelective IDs (the selective catalog, not the global CatalogYear)
        if (query.CatalogYearIds != null && query.CatalogYearIds.Any())
            bindsQuery = bindsQuery.Where(b => query.CatalogYearIds.Contains(b.SelectiveDiscipline.CatalogId));

        var binds = await bindsQuery.ToListAsync();

        var items = binds.Select(b => new StudentOwnSelectiveDisciplineItemDto
        {
            BindId                = b.IdBindSelectiveDisciplines,
            SelectiveDisciplineId = b.SelectiveDisciplineId,
            Name                  = b.SelectiveDiscipline.NameSelectiveDisciplines,
            Code                  = b.SelectiveDiscipline.CodeSelectiveDisciplines,
            Semestr               = b.Semestr,
            Loans                 = b.Loans,
            Grade                 = b.Grade,
            InProcess             = b.InProcess,
            IsRejected            = b.IsRejected,
            CatalogYearId         = b.SelectiveDiscipline.CatalogId,
            CatalogYearStart      = b.SelectiveDiscipline.Catalog.YearStart,
            CatalogYearEnd        = b.SelectiveDiscipline.Catalog.YearEnd,
            DepartmentName        = b.SelectiveDiscipline.Department?.NameDepartment,
            FacultyAbbreviation   = b.SelectiveDiscipline.Department?.Faculty?.Abbreviation
        }).ToList();

        return new StudentOwnSelectiveDisciplinesDto
        {
            StudentId            = student.IdStudent,
            FirstName            = student.FirstName,
            SecondName           = student.SecondName ?? string.Empty,
            ThirdName            = student.ThirdName ?? string.Empty,
            SelectiveDisciplines = items
        };
    }

    public async Task<StudentEducationalProgramWithGradesDto?> GetStudentEducationalProgramWithGradesAsync(Guid studentId)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.Group)
                .ThenInclude(g => g.EducationalProgram)
                    .ThenInclude(ep => ep.MainDisciplines)
                        .ThenInclude(md => md.TypeOfControlNavigation)
            .FirstOrDefaultAsync(s => s.IdStudent == studentId);

        if (student == null) return null;

        var ep = student.Group.EducationalProgram;

        // Load all grade binds for this student in a single round-trip
        var gradesByDisciplineId = await _context.BindMainDisciplines
            .AsNoTracking()
            .Where(b => b.StudentId == studentId)
            .ToListAsync();

        // Index: disciplineId -> latest bind (take highest grade in case of redo)
        var gradeMap = gradesByDisciplineId
            .GroupBy(b => b.MainDisciplinesId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(b => b.Grade).First());

        var disciplinesBySemester = ep.MainDisciplines
            .OrderBy(md => md.Semestr)
            .ThenBy(md => md.CodeMainDisciplines)
            .GroupBy(md => md.Semestr)
            .ToDictionary(
                g => g.Key,
                g => g.Select(md =>
                {
                    gradeMap.TryGetValue(md.IdMainDisciplines, out var bind);
                    return new MainDisciplineWithGradeDto
                    {
                        IdMainDisciplines = md.IdMainDisciplines,
                        Code              = md.CodeMainDisciplines,
                        Name              = md.NameMainDisciplines,
                        Semestr           = md.Semestr,
                        Loans             = md.Loans,
                        Hours             = md.Hours,
                        TypeOfControlName = md.TypeOfControlNavigation?.Type,
                        Grade             = bind?.Grade,
                        IsRedo            = bind?.IsRedo
                    };
                }).ToList());

        return new StudentEducationalProgramWithGradesDto
        {
            StudentId              = student.IdStudent,
            FirstName              = student.FirstName,
            SecondName             = student.SecondName ?? string.Empty,
            ThirdName              = student.ThirdName ?? string.Empty,
            EducationalProgramId   = ep.IdEducationalProgram,
            EducationalProgramName = ep.NameEducationalProgram,
            DisciplinesBySemester  = disciplinesBySemester
        };
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
