using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDisciplineTabAdminRepository
{
    Task<(int TotalCount, List<FullDisciplineDto> Items)> GetAllDisciplinesPagedAsync(GetAllDisciplinesAdminQueryDto queryDto);
    Task<DateTime?> GetLastPeriodStartDateAsync(int facultyId);
    /// <summary>End date of the most recent discipline choice period for the faculty that has already ended (null if none).</summary>
    Task<DateTime?> GetLastCompletedPeriodEndDateAsync(int facultyId);
    Task<List<StudentChoicesProjection>> GetStudentsChoicesForFacultyAsync(int facultyId);
    Task<List<StudentChoicesProjection>> GetStudentsChoicesDataAsync(GetStudentsWithDisciplineChoicesQueryDto queryDto, DateTime? periodStart);
    Task<Dictionary<int, DateTime>> GetLastPeriodsByFacultyAsync();
    Task<Dictionary<(int Level, sbyte IsFaculty), int>> GetNormativesLookupAsync();
    Task<List<DisciplineStatusProjection>> GetDisciplinesStatusDataAsync(GetDisciplinesWithStatusQueryDto queryDto);
    Task<Dictionary<int, BindSelectiveDiscipline>> GetBindsWithDetailsAsync(List<int> bindIds);
    Task<BindSelectiveDisciplineDto?> GetBindDtoAsync(int id);
    Task<StudentChoicesProjection?> GetStudentChoicesDataAsync(int studentId);
    Task<bool> ExistsBindAsync(int studentId, int disciplineId);
    Task<bool> ExistsStudentAsync(int studentId);
    Task<bool> ExistsDisciplineAsync(int disciplineId);
    Task AddBindAsync(BindSelectiveDiscipline bind);
    Task<int> DeleteBindAsync(int id);
    Task<SelectiveDiscipline?> GetDisciplineEntityAsync(int id);
    void RemoveBind(BindSelectiveDiscipline bind);
    void AddNotification(Notification notification);

    Task<BindSelectiveDiscipline?> GetBindByStudentAndDisciplineAsync(int studentId, int disciplineId);
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
        var query = _context.SelectiveDisciplines.AsNoTracking().AsQueryable();

        // 1. ?ќќќќќќќќ ќќ ќќќќќќ
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        // 2. ?ќќќќќќќќ ќќ ќќќќќќќќќќќ
        if (queryDto.Faculties != null && queryDto.Faculties.Any())
        {
            query = query.Where(d => d.FacultyId.HasValue && queryDto.Faculties.Contains(d.FacultyId.Value));
        }

        // 3. ?ќќќќќќќќ ќќ ќќќќќќќќќќќ
        if (queryDto.Courses != null && queryDto.Courses.Any())
        {
            query = query.Where(d =>
                (!d.MinCourse.HasValue || queryDto.Courses.Contains(d.MinCourse.Value)) &&
                (!d.MaxCourse.HasValue || queryDto.Courses.Contains(d.MaxCourse.Value)));
        }

        // 4. ?ќќќќќќќќ ќќ ќќќќќќќќ
        if (queryDto.IsEvenSemester.HasValue)
        {
            query = query.Where(d => d.IsEven.HasValue &&
                ((queryDto.IsEvenSemester.Value && d.IsEven.Value % 2 == 0) ||
                 (!queryDto.IsEvenSemester.Value && d.IsEven.Value % 2 == 1)));
        }

        // 5. ?ќќќќќќќќ ќќ ќќќќќќќќ
        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));
        }

        // ?ќќќќќќќќ ќќќќќќќќ ќќќќҡКќќ ќќ ќќќќќќќќ
        var totalCount = await query.CountAsync();

        // 6. ќќќќќќќќќќ ќќ ќќќќ ќќ
        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(d => d.NameSelectiveDisciplines),
            2 => query.OrderByDescending(d => d.NameSelectiveDisciplines),
            3 => query.OrderBy(d => d.CodeSelectiveDisciplines),
            4 => query.OrderByDescending(d => d.CodeSelectiveDisciplines),
            _ => query.OrderBy(d => d.IdSelectiveDisciplines)
        };

        // 7. ќќќќќќќќ (ќќќќ: EF Core ќќќќќќќќќќќ BindSelectiveDisciplines.Count ќќ ќќќќќќќ SELECT COUNT(*))
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(d => new FullDisciplineDto
            {
                IdSelectiveDisciplines = d.IdSelectiveDisciplines,
                NameSelectiveDisciplines = d.NameSelectiveDisciplines ?? "",
                CodeSelectiveDisciplines = d.CodeSelectiveDisciplines ?? "",
                FacultyId = d.FacultyId ?? 0,
                FacultyAbbreviation = d.Faculty != null ? d.Faculty.Abbreviation ?? "" : "",
                MaxCountPeople = d.MaxCountPeople,
                MinCourse = d.MinCourse,
                MaxCourse = d.MaxCourse,
                IsEven = d.IsEven.HasValue ? (sbyte?)d.IsEven.Value : null,
                DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegreec ?? "" : "",
                CountOfPeople = d.BindSelectiveDisciplines.Count, // ?ќќќќ ќќќќќќ Include!
                IsAvailable = false // ќќќ ќќќќ-ќќќќќ ќќ ќќќќ ќќќќќќќќ ќќќќќќќќќќќ, ќќќ ќќќќќ ќќќќќќќ true
            })
            .ToListAsync();

        return (totalCount, items);
    }
    public async Task<DateTime?> GetLastPeriodStartDateAsync(int facultyId)
    {
        var raw = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.FacultyId == facultyId)
            .OrderByDescending(p => p.StartDate)
            .Select(p => p.StartDate)
            .FirstOrDefaultAsync();
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        return DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt)
            ? dt
            : null;
    }

    public async Task<DateTime?> GetLastCompletedPeriodEndDateAsync(int facultyId)
    {
        var now = DateTime.UtcNow;
        var rows = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.FacultyId == facultyId && p.EndDate != null)
            .Select(p => p.EndDate!)
            .ToListAsync();
        DateTime? best = null;
        foreach (var s in rows)
        {
            if (!DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var end))
                continue;
            if (end >= now)
                continue;
            if (best == null || end > best)
                best = end;
        }
        return best;
    }

    public async Task<List<StudentChoicesProjection>> GetStudentsChoicesForFacultyAsync(int facultyId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.FacultyId == facultyId)
            .Select(s => new StudentChoicesProjection(
                s.IdStudent,
                s.NameStudent ?? "",
                s.Faculty != null ? s.Faculty.Abbreviation ?? s.Faculty.NameFaculty : "",
                s.Group != null ? s.Group.GroupCode : "",
                s.Course,
                s.EducationalDegreeId,
                s.EducationalDegree != null ? s.EducationalDegree.NameEducationalDegreec : "",
                s.EducationalProgram,
                s.BindSelectiveDisciplines.Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                    IdSelectiveDisciplines = b.SelectiveDisciplinesId ?? 0,
                    NameSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.NameSelectiveDisciplines ?? "" : "",
                    CodeSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.CodeSelectiveDisciplines ?? "" : "",
                    Semestr = b.Semestr ?? 0,
                    InProcess = (sbyte)(b.InProcess != null && b.InProcess.Length > 0 && b.InProcess[0] ? (sbyte)1 : (sbyte)0)
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
                (s.Faculty != null && (EF.Functions.Like(s.Faculty.NameFaculty.ToLower(), $"%{lowerSearch}%") || EF.Functions.Like(s.Faculty.Abbreviation.ToLower(), $"%{lowerSearch}%"))) ||
                (s.Group != null && EF.Functions.Like(s.Group.GroupCode.ToLower(), $"%{lowerSearch}%")));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(s => queryDto.Faculties.Contains(s.FacultyId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(s => queryDto.Courses.Contains(s.Course));

        if (queryDto.StudentGroups != null && queryDto.StudentGroups.Any())
            query = query.Where(s => queryDto.StudentGroups.Contains(s.GroupId));

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(s => queryDto.DegreeLevelIds.Contains(s.EducationalDegreeId));

        if (queryDto.IsNew == 1 && queryDto.FacultyId > 0)
            query = query.Where(s => s.FacultyId == queryDto.FacultyId);

        return await query.Select(s => new StudentChoicesProjection(
            s.IdStudent,
            s.NameStudent ?? "",
            s.Faculty != null ? s.Faculty.Abbreviation ?? s.Faculty.NameFaculty : "",
            s.Group != null ? s.Group.GroupCode : "",
            s.Course,
            s.EducationalDegreeId,
            s.EducationalDegree != null ? s.EducationalDegree.NameEducationalDegreec : "",
            s.EducationalProgram,
            s.BindSelectiveDisciplines
                .Where(b => queryDto.IsNew == 0 || periodStart == null)
                .Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                    IdSelectiveDisciplines = b.SelectiveDisciplinesId ?? 0,
                    NameSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.NameSelectiveDisciplines ?? "" : "",
                    CodeSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.CodeSelectiveDisciplines ?? "" : "",
                    Semestr = b.Semestr ?? 0,
                    InProcess = (sbyte)(b.InProcess != null && b.InProcess.Length > 0 && b.InProcess[0] ? (sbyte)1 : (sbyte)0)
                }).ToList()
        )).ToListAsync();
    }

    public async Task<Dictionary<int, DateTime>> GetLastPeriodsByFacultyAsync()
    {
        var rows = await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.FacultyId != null && p.StartDate != null)
            .Select(p => new { FacultyId = p.FacultyId!.Value, p.StartDate })
            .ToListAsync();

        var dict = new Dictionary<int, DateTime>();
        foreach (var g in rows.GroupBy(r => r.FacultyId))
        {
            DateTime? best = null;
            foreach (var r in g)
            {
                if (!DateTime.TryParse(r.StartDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
                    continue;
                if (best == null || dt > best)
                    best = dt;
            }
            if (best.HasValue)
                dict[g.Key] = best.Value;
        }
        return dict;
    }

    public async Task<Dictionary<(int Level, sbyte IsFaculty), int>> GetNormativesLookupAsync()
    {
        return await _context.Normatives
            .Where(n => n.DegreeLevelId != null && n.IsFaculty != null)
            .GroupBy(n => new { Level = n.DegreeLevelId!.Value, IsFaculty = (sbyte)((n.IsFaculty != null && n.IsFaculty.Length > 0 && n.IsFaculty[0]) ? 1 : 0) })
            .ToDictionaryAsync(
                g => (g.Key.Level, g.Key.IsFaculty),
                g => g.First().Count.GetValueOrDefault()
            );
    }

    public async Task<List<DisciplineStatusProjection>> GetDisciplinesStatusDataAsync(GetDisciplinesWithStatusQueryDto queryDto)
    {
        var query = _context.SelectiveDisciplines.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d => EF.Functions.Like(d.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") || EF.Functions.Like(d.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => d.FacultyId.HasValue && queryDto.Faculties.Contains(d.FacultyId.Value));

        if (queryDto.IsFaculty.HasValue)
            query = query.Where(d => d.IsFaculty == queryDto.IsFaculty.Value);

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));

        return await query.Select(d => new DisciplineStatusProjection(
            d.IdSelectiveDisciplines,
            d.NameSelectiveDisciplines ?? "",
            d.SelectiveDetail != null ? d.SelectiveDetail.Teachers : null,
            d.SelectiveDetail != null && d.SelectiveDetail.Department != null ? d.SelectiveDetail.Department.NameDepartment : null,
            d.MinCountPeople,
            d.MaxCountPeople,
            (sbyte)(d.IsForseChange ?? 0),
            d.Type != null ? d.Type.TypeName ?? "" : "",
            d.DegreeLevelId,
            (sbyte)(d.IsFaculty ?? 0),
            d.FacultyId ?? 0,
            d.Faculty != null ? d.Faculty.Abbreviation : null,
            d.BindSelectiveDisciplines.Select(b => b.CreatedAt).ToList()
        )).ToListAsync();
    }

    public async Task<Dictionary<int, BindSelectiveDiscipline>> GetBindsWithDetailsAsync(List<int> bindIds)
    {
        return await _context.BindSelectiveDisciplines
            .Include(b => b.Student)
            .Include(b => b.SelectiveDisciplines)
            .Where(b => b.IdBindSelectiveDisciplines != null && bindIds.Contains(b.IdBindSelectiveDisciplines))
            .ToDictionaryAsync(b => b.IdBindSelectiveDisciplines!);
    }

    public async Task<BindSelectiveDisciplineDto?> GetBindDtoAsync(int id)
    {
        return await _context.BindSelectiveDisciplines
            .AsNoTracking()
            .Where(b => b.IdBindSelectiveDisciplines == id)
            .Select(b => new BindSelectiveDisciplineDto
            {
                IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                StudentId = b.StudentId ?? 0,
                StudentFullName = b.Student != null ? b.Student.NameStudent ?? "" : "",
                SelectiveDisciplinesId = b.SelectiveDisciplinesId ?? 0,
                SelectiveDisciplineName = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.NameSelectiveDisciplines ?? "" : "",
                Semestr = b.Semestr ?? 0,
                Loans = b.Loans ?? 0,
                InProcess = b.InProcess != null && b.InProcess.Length > 0 && b.InProcess[0]
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StudentChoicesProjection?> GetStudentChoicesDataAsync(int studentId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId)
            .Select(s => new StudentChoicesProjection(
                s.IdStudent,
                s.NameStudent ?? "",
                s.Faculty != null ? s.Faculty.Abbreviation ?? s.Faculty.NameFaculty : "",
                s.Group != null ? s.Group.GroupCode : "",
                s.Course,
                s.EducationalDegreeId,
                s.EducationalDegree != null ? s.EducationalDegree.NameEducationalDegreec : "",
                s.EducationalProgram,
                s.BindSelectiveDisciplines.Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindSelectiveDisciplines = b.IdBindSelectiveDisciplines,
                    IdSelectiveDisciplines = b.SelectiveDisciplinesId ?? 0,
                    NameSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.NameSelectiveDisciplines ?? "" : "",
                    CodeSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.CodeSelectiveDisciplines ?? "" : "",
                    Semestr = b.Semestr ?? 0,
                    InProcess = (sbyte)(b.InProcess != null && b.InProcess.Length > 0 && b.InProcess[0] ? (sbyte)1 : (sbyte)0)
                }).ToList()
            )).FirstOrDefaultAsync();
    }

    public async Task<BindSelectiveDiscipline?> GetBindByStudentAndDisciplineAsync(int studentId, int disciplineId)
    {
        return await _context.BindSelectiveDisciplines
            .Include(b => b.Student)
            .Include(b => b.SelectiveDisciplines)
            .FirstOrDefaultAsync(b => b.StudentId == studentId && b.SelectiveDisciplinesId == disciplineId);
    }

    public async Task<bool> ExistsBindAsync(int studentId, int disciplineId) =>
        await _context.BindSelectiveDisciplines.AnyAsync(b => b.StudentId == studentId && b.SelectiveDisciplinesId == disciplineId);

    public async Task<bool> ExistsStudentAsync(int studentId) =>
        await _context.Students.AnyAsync(s => s.IdStudent == studentId);

    public async Task<bool> ExistsDisciplineAsync(int disciplineId) =>
        await _context.SelectiveDisciplines.AnyAsync(d => d.IdSelectiveDisciplines == disciplineId);

    public async Task AddBindAsync(BindSelectiveDiscipline bind) =>
        await _context.BindSelectiveDisciplines.AddAsync(bind);

    public async Task<int> DeleteBindAsync(int id) =>
        await _context.BindSelectiveDisciplines.Where(b => b.IdBindSelectiveDisciplines == id).ExecuteDeleteAsync();

    public async Task<SelectiveDiscipline?> GetDisciplineEntityAsync(int id) =>
        await _context.SelectiveDisciplines.FindAsync(id);

    public void RemoveBind(BindSelectiveDiscipline bind) => _context.BindSelectiveDisciplines.Remove(bind);

    public void AddNotification(Notification notification) => _context.Notifications.Add(notification);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}