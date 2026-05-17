using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDisciplineTabAdminRepository
{
    Task<(int TotalCount, List<FullDisciplineDto> Items)> GetAllDisciplinesPagedAsync(GetAllDisciplinesAdminQueryDto queryDto);
    Task<DateTime?> GetLastPeriodStartDateAsync(Guid facultyId);
    /// <summary>End date of the most recent discipline choice period for the faculty that has already ended (null if none).</summary>
    Task<DateTime?> GetLastCompletedPeriodEndDateAsync(Guid facultyId);
    Task<List<StudentChoicesProjection>> GetStudentsChoicesForFacultyAsync(Guid facultyId);
    Task<List<StudentChoicesProjection>> GetStudentsChoicesDataAsync(GetStudentsWithDisciplineChoicesQueryDto queryDto, DateTime? periodStart);
    Task<Dictionary<Guid, DateTime>> GetLastPeriodsByFacultyAsync();
    Task<Dictionary<(Guid Level, bool IsFaculty), int>> GetNormativesLookupAsync();
    Task<List<DisciplineStatusProjection>> GetDisciplinesStatusDataAsync(GetDisciplinesWithStatusQueryDto queryDto);
    Task<Dictionary<Guid, BindSelectiveDiscipline>> GetBindsWithDetailsAsync(List<Guid> bindIds);
    Task<BindSelectiveDisciplineDto?> GetBindDtoAsync(Guid id);
    Task<StudentChoicesProjection?> GetStudentChoicesDataAsync(Guid studentId);
    Task<bool> ExistsBindAsync(Guid studentId, Guid disciplineId);
    Task<bool> ExistsStudentAsync(Guid studentId);
    Task<bool> ExistsDisciplineAsync(Guid disciplineId);
    Task AddBindAsync(BindSelectiveDiscipline bind);
    Task<int> DeleteBindAsync(Guid id);
    Task<SelectiveDiscipline?> GetDisciplineEntityAsync(Guid id);
    void RemoveBind(BindSelectiveDiscipline bind);
    void AddNotification(Notification notification);

    Task<BindSelectiveDiscipline?> GetBindByStudentAndDisciplineAsync(Guid studentId, Guid disciplineId);
    Task SaveChangesAsync();
}

public class DisciplineTabAdminRepository : IDisciplineTabAdminRepository
{
    private readonly AppDbContext _context;

    public DisciplineTabAdminRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<(int TotalCount, List<FullDisciplineDto> Items)> GetAllDisciplinesPagedAsync(GetAllDisciplinesAdminQueryDto queryDto)
    {
        var query = _context.SelectiveDisciplines
            .Include(d => d.SelectiveDetail)
            .AsNoTracking()
            .AsQueryable();

        // 1. Search
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                (d.SelectiveDetail != null && EF.Functions.Like(d.SelectiveDetail.Teachers.ToLower(), $"%{lowerSearch}%")));
        }

        // 2. Faculties
        if (queryDto.Faculties != null && queryDto.Faculties.Any())
        {
            query = query.Where(d => d.Department.FacultyId != null && queryDto.Faculties.Contains(d.Department.FacultyId));
        }

        // 3. Courses
        if (queryDto.Courses != null && queryDto.Courses.Any())
        {
            query = query.Where(d => (!d.Courses.Any() || queryDto.Courses.Any(c => d.Courses.Contains(c))));
        }

        // 4. IsEven
        if (queryDto.IsEvenSemester.HasValue)
        {
            if (queryDto.IsEvenSemester.Value) // Paired
            {
                query = query.Where(d => d.IsEven == null || d.IsEven == true);
            }
            else // Unpaired
            {
                query = query.Where(d => d.IsEven == null || d.IsEven == false);
            }
        }

        // 5. DegreeLevel
        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(d => d.DegreeLevelId != Guid.Empty && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId));
        }

        // TypeOfControl
        if (queryDto.TypeOfControlIds != null && queryDto.TypeOfControlIds.Any())
        {
            query = query.Where(d => d.TypeOfControlId != Guid.Empty && queryDto.TypeOfControlIds.Contains(d.TypeOfControlId));
        }

        // ApprovalStatus
        if (queryDto.ApprovalStatusIds != null && queryDto.ApprovalStatusIds.Any())
        {
            query = query.Where(d => d.ApprovalStatusId != Guid.Empty && queryDto.ApprovalStatusIds.Contains(d.ApprovalStatusId));
        }

        var totalCount = await query.CountAsync();

        // 6. Sorting
        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(d => d.NameSelectiveDisciplines),
            2 => query.OrderByDescending(d => d.NameSelectiveDisciplines),
            3 => query.OrderBy(d => d.CodeSelectiveDisciplines),
            4 => query.OrderByDescending(d => d.CodeSelectiveDisciplines),
            _ => query.OrderBy(d => d.IdSelectiveDisciplines)
        };

        // 7. Pagination and Projection
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(d => new FullDisciplineDto
            {
                IdSelectiveDisciplines = d.IdSelectiveDisciplines,
                NameSelectiveDisciplines = d.NameSelectiveDisciplines ?? "",
                CodeSelectiveDisciplines = d.CodeSelectiveDisciplines ?? "",
                FacultyId = d.Department.FacultyId,
                FacultyAbbreviation = d.Department.Faculty != null ? d.Department.Faculty.Abbreviation ?? "" : "",
                MaxCountPeople = d.MaxCountPeople,
                Courses = d.Courses ?? new List<int>(),
                IsEven = d.IsEven,
                DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegree ?? "" : "",
                CountOfPeople = d.BindSelectiveDisciplines.Count,
                IsAvailable = false,
                CatalogId = d.CatalogId,
                ApprovalStatusId = d.ApprovalStatusId,
                TypeOfControlId = d.TypeOfControlId
            })
            .ToListAsync();

        return (totalCount, items);
    }
    public async Task<DateTime?> GetLastPeriodStartDateAsync(Guid facultyId)
    {
        var raw = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.Department != null && p.Department.FacultyId == facultyId)
            .OrderByDescending(p => p.StartDate)
            .Select(p => (DateOnly?)p.StartDate)
            .FirstOrDefaultAsync();
        if (raw == null)
            return null;

        return raw.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
    }

    public async Task<DateTime?> GetLastCompletedPeriodEndDateAsync(Guid facultyId)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var rows = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.Department != null && p.Department.FacultyId == facultyId && p.EndDate != null)
            .Select(p => p.EndDate!)
            .ToListAsync();
        DateOnly? best = null;
        foreach (var end in rows)
        {
            if (end >= now)
                continue;
            if (best == null || end > best)
                best = end;
        }
        return best?.ToDateTime(TimeOnly.MinValue);
    }

    public async Task<List<StudentChoicesProjection>> GetStudentsChoicesForFacultyAsync(Guid facultyId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.Group.EducationalProgram.Speciality.Department.FacultyId == facultyId)
            .Select(s => new StudentChoicesProjection(
                s.IdStudent,
                s.NameStudent ?? "",
                s.Group.EducationalProgram.Speciality.Department.Faculty != null ? s.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation ?? s.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty : "",
                s.Group != null ? s.Group.GroupCode : "",
                s.Group != null ? s.Group.Course : 0,
                s.Group.EducationalProgram != null ? s.Group.EducationalProgram.DegreeId : Guid.Empty,
                s.Group.EducationalProgram != null && s.Group.EducationalProgram.Degree != null ? s.Group.EducationalProgram.Degree.NameEducationalDegree : "",
                s.Group.EducationalProgram,
                s.BindSelectiveDisciplines.Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                    IdSelectiveDisciplines = b.SelectiveDisciplineId != Guid.Empty ? b.SelectiveDisciplineId : Guid.Empty,
                    NameSelectiveDisciplines = b.SelectiveDiscipline != null ? b.SelectiveDiscipline.NameSelectiveDisciplines ?? "" : "",
                    CodeSelectiveDisciplines = b.SelectiveDiscipline != null ? b.SelectiveDiscipline.CodeSelectiveDisciplines ?? "" : "",
                    Semestr = b.Semestr >= 0 ? b.Semestr : 0,
                    InProcess = b.InProcess
                }).ToList()
            ))
            .ToListAsync();
    }

    public async Task<List<StudentChoicesProjection>> GetStudentsChoicesDataAsync(GetStudentsWithDisciplineChoicesQueryDto queryDto, DateTime? periodStart)
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s =>
                EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%") ||
                (s.Group.EducationalProgram.Speciality.Department.Faculty != null && (EF.Functions.Like(s.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty.ToLower(), $"%{lowerSearch}%") || EF.Functions.Like(s.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation.ToLower(), $"%{lowerSearch}%"))) ||
                (s.Group != null && EF.Functions.Like(s.Group.GroupCode.ToLower(), $"%{lowerSearch}%")));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(s => s.Group.EducationalProgram.Speciality.Department.FacultyId != null && queryDto.Faculties.Contains(s.Group.EducationalProgram.Speciality.Department.FacultyId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(s => queryDto.Courses.Contains(s.Group.Course));

        if (queryDto.StudentGroups != null && queryDto.StudentGroups.Any())
            query = query.Where(s => s.GroupId != null && queryDto.StudentGroups.Contains(s.GroupId));

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(s => s.Group.EducationalProgram != null && s.Group.EducationalProgram.DegreeId != null && queryDto.DegreeLevelIds.Contains(s.Group.EducationalProgram.DegreeId));

        if (queryDto.IsNew && queryDto.FacultyId != Guid.Empty)
            query = query.Where(s => s.Group.EducationalProgram.Speciality.Department.FacultyId == queryDto.FacultyId);

        return await query.Select(s => new StudentChoicesProjection(
            s.IdStudent,
            s.NameStudent ?? "",
            s.Group.EducationalProgram.Speciality.Department.Faculty != null ? s.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation ?? s.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty : "",
            s.Group != null ? s.Group.GroupCode : "",
            s.Group != null ? s.Group.Course : 0,
            s.Group.EducationalProgram != null ? s.Group.EducationalProgram.DegreeId : Guid.Empty,
            s.Group.EducationalProgram != null && s.Group.EducationalProgram.Degree != null ? s.Group.EducationalProgram.Degree.NameEducationalDegree : "",
            s.Group.EducationalProgram,
            s.BindSelectiveDisciplines
                .Where(b => !queryDto.IsNew || periodStart == null)
                .Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                    IdSelectiveDisciplines = b.SelectiveDisciplineId != Guid.Empty ? b.SelectiveDisciplineId : Guid.Empty,
                    NameSelectiveDisciplines = b.SelectiveDiscipline != null ? b.SelectiveDiscipline.NameSelectiveDisciplines ?? "" : "",
                    CodeSelectiveDisciplines = b.SelectiveDiscipline != null ? b.SelectiveDiscipline.CodeSelectiveDisciplines ?? "" : "",
                    Semestr = b.Semestr >= 0 ? b.Semestr : 0,
                    InProcess = b.InProcess
                }).ToList()
        )).ToListAsync();
    }

    public async Task<Dictionary<Guid, DateTime>> GetLastPeriodsByFacultyAsync()
    {
        var rows = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.Department != null && p.Department.FacultyId != null && p.StartDate != null)
            .Select(p => new { FacultyId = p.Department.FacultyId!, StartDate = p.StartDate })
            .ToListAsync();

        var dict = new Dictionary<Guid, DateTime>();
        foreach (var g in rows.GroupBy(r => r.FacultyId))
        {
            DateOnly? best = null;
            foreach (var r in g)
            {
                if (best == null || r.StartDate > best)
                    best = r.StartDate;
            }
            if (best.HasValue)
                dict[g.Key] = best.Value.ToDateTime(TimeOnly.MinValue);
        }
        return dict;
    }

    public async Task<Dictionary<(Guid Level, bool IsFaculty), int>> GetNormativesLookupAsync()
    {
        return await _context.Normatives
            .Where(n => n.DegreeLevelId != null)
            .GroupBy(n => new { Level = n.DegreeLevelId!, IsFaculty = n.IsFaculty})
            .ToDictionaryAsync(
                g => (g.Key.Level, g.Key.IsFaculty),
                g => g.First().Count
            );
    }

    public async Task<List<DisciplineStatusProjection>> GetDisciplinesStatusDataAsync(GetDisciplinesWithStatusQueryDto queryDto)
    {
        var query = _context.SelectiveDisciplines.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d => 
                EF.Functions.Like(d.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") || 
                EF.Functions.Like(d.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                (d.SelectiveDetail != null && EF.Functions.Like(d.SelectiveDetail.Teachers.ToLower(), $"%{lowerSearch}%")));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => d.Department.FacultyId != null && queryDto.Faculties.Contains(d.Department.FacultyId));

        if (queryDto.IsFaculty.HasValue)
            query = query.Where(d => d.IsFaculty == queryDto.IsFaculty.Value);

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId != Guid.Empty && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId));

        if (queryDto.TypeOfControlIds != null && queryDto.TypeOfControlIds.Any())
            query = query.Where(d =>d.TypeOfControlId != Guid.Empty && queryDto.TypeOfControlIds.Contains(d.TypeOfControlId));

        if (queryDto.ApprovalStatusIds != null && queryDto.ApprovalStatusIds.Any())
            query = query.Where(d =>d.ApprovalStatusId != Guid.Empty && queryDto.ApprovalStatusIds.Contains(d.ApprovalStatusId));

        return await query.Select(d => new DisciplineStatusProjection(
            d.IdSelectiveDisciplines,
            d.NameSelectiveDisciplines ?? "",
            d.SelectiveDetail != null ? d.SelectiveDetail.Teachers : null,
            d.Department != null ? d.Department.NameDepartment : null,
            d.MinCountPeople,
            d.MaxCountPeople,
            d.IsForseChange == true,
            d.Type != null ? d.Type.TypeName ?? "" : "",
            d.DegreeLevelId,
            d.IsFaculty,
            d.Department.FacultyId,
            d.Department.Faculty != null ? d.Department.Faculty.Abbreviation : null,
            d.BindSelectiveDisciplines.Select(b => b.CreatedAt.ToString()).ToList()
        )).ToListAsync();
    }

    public async Task<Dictionary<Guid, BindSelectiveDiscipline>> GetBindsWithDetailsAsync(List<Guid> bindIds)
    {
        return await _context.BindSelectiveDisciplines
            .Include(b => b.Student)
            .Include(b => b.SelectiveDiscipline)
            .Where(b => bindIds.Contains(b.IdBindSelectiveDisciplines))
            .ToDictionaryAsync(b => b.IdBindSelectiveDisciplines);
    }

    public async Task<BindSelectiveDisciplineDto?> GetBindDtoAsync(Guid id)
    {
        return await _context.BindSelectiveDisciplines
            .AsNoTracking()
            .Where(b => b.IdBindSelectiveDisciplines == id)
            .Select(b => new BindSelectiveDisciplineDto
            {
                IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                StudentId = b.StudentId,
                StudentFullName = b.Student != null ? b.Student.NameStudent ?? "" : "",
                SelectiveDisciplinesId = b.SelectiveDisciplineId,
                SelectiveDisciplineName = b.SelectiveDiscipline != null ? b.SelectiveDiscipline.NameSelectiveDisciplines ?? "" : "",
                Semestr = b.Semestr,
                Loans = b.Loans ?? 0,
                InProcess = b.InProcess
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StudentChoicesProjection?> GetStudentChoicesDataAsync(Guid studentId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId)
            .Select(s => new StudentChoicesProjection(
                s.IdStudent,
                s.NameStudent ?? "",
                s.Group != null && s.Group.EducationalProgram != null && s.Group.EducationalProgram.Speciality != null && s.Group.EducationalProgram.Speciality.Department != null && s.Group.EducationalProgram.Speciality.Department.Faculty != null
                    ? s.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation ?? s.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty
                    : "",
                s.Group != null ? s.Group.GroupCode : "",
                s.Group != null ? s.Group.Course : 0,
                s.Group != null ? s.Group.EducationalProgram != null ? s.Group.EducationalProgram.DegreeId : Guid.Empty : Guid.Empty,
                s.Group != null && s.Group.EducationalProgram != null && s.Group.EducationalProgram.Degree != null ? s.Group.EducationalProgram.Degree.NameEducationalDegree : "",
                s.Group != null ? s.Group.EducationalProgram : null,
                s.BindSelectiveDisciplines.Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                    IdSelectiveDisciplines = b.SelectiveDisciplineId != Guid.Empty ? b.SelectiveDisciplineId : Guid.Empty,
                    NameSelectiveDisciplines = b.SelectiveDiscipline != null ? b.SelectiveDiscipline.NameSelectiveDisciplines ?? "" : "",
                    CodeSelectiveDisciplines = b.SelectiveDiscipline != null ? b.SelectiveDiscipline.CodeSelectiveDisciplines ?? "" : "",
                    Semestr = b.Semestr >= 0 ? b.Semestr : 0,
                    InProcess = b.InProcess
                }).ToList()
            )).FirstOrDefaultAsync();
    }

    public async Task<BindSelectiveDiscipline?> GetBindByStudentAndDisciplineAsync(Guid studentId, Guid disciplineId)
    {
        return await _context.BindSelectiveDisciplines
            .Include(b => b.Student)
            .Include(b => b.SelectiveDiscipline)
            .FirstOrDefaultAsync(b => b.StudentId == studentId && b.SelectiveDisciplineId == disciplineId);
    }

    public async Task<bool> ExistsBindAsync(Guid studentId, Guid disciplineId) =>
        await _context.BindSelectiveDisciplines.AnyAsync(b => b.StudentId == studentId && b.SelectiveDisciplineId == disciplineId);

    public async Task<bool> ExistsStudentAsync(Guid studentId) =>
        await _context.Students.AnyAsync(s => s.IdStudent == studentId);

    public async Task<bool> ExistsDisciplineAsync(Guid disciplineId) =>
        await _context.SelectiveDisciplines.AnyAsync(d => d.IdSelectiveDisciplines == disciplineId);

    public async Task AddBindAsync(BindSelectiveDiscipline bind) =>
        await _context.BindSelectiveDisciplines.AddAsync(bind);

    public async Task<int> DeleteBindAsync(Guid id) =>
        await _context.BindSelectiveDisciplines.Where(b => b.IdBindSelectiveDisciplines == id).ExecuteDeleteAsync();

    public async Task<SelectiveDiscipline?> GetDisciplineEntityAsync(Guid id) =>
        await _context.SelectiveDisciplines.FindAsync(id);

    public void RemoveBind(BindSelectiveDiscipline bind) => _context.BindSelectiveDisciplines.Remove(bind);

    public void AddNotification(Notification notification) => _context.Notifications.Add(notification);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}