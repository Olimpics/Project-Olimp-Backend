using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;

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
    Task<Dictionary<int, BindAddDiscipline>> GetBindsWithDetailsAsync(List<int> bindIds);
    Task<BindAddDisciplineDto?> GetBindDtoAsync(int id);
    Task<StudentChoicesProjection?> GetStudentChoicesDataAsync(int studentId);
    Task<bool> ExistsBindAsync(int studentId, int disciplineId);
    Task<bool> ExistsStudentAsync(int studentId);
    Task<bool> ExistsDisciplineAsync(int disciplineId);
    Task AddBindAsync(BindAddDiscipline bind);
    Task<int> DeleteBindAsync(int id);
    Task<AddDiscipline?> GetDisciplineEntityAsync(int id);
    void RemoveBind(BindAddDiscipline bind);
    void AddNotification(Notification notification);

    Task<BindAddDiscipline?> GetBindByStudentAndDisciplineAsync(int studentId, int disciplineId);
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
        var query = _context.AddDisciplines.AsNoTracking().AsQueryable();

        // 1. ?ùùùùùùùù ùù ùùùùùù
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        // 2. ?ùùùùùùùù ùù ùùùùùùùùùùù
        if (queryDto.Faculties != null && queryDto.Faculties.Any())
        {
            query = query.Where(d => d.FacultyId.HasValue && queryDto.Faculties.Contains(d.FacultyId.Value));
        }

        // 3. ?ùùùùùùùù ùù ùùùùùù
        if (queryDto.Courses != null && queryDto.Courses.Any())
        {
            query = query.Where(d =>
                (!d.MinCourse.HasValue || queryDto.Courses.Contains(d.MinCourse.Value)) &&
                (!d.MaxCourse.HasValue || queryDto.Courses.Contains(d.MaxCourse.Value)));
        }

        // 4. ?ùùùùùùùù ùù ùùùùùùùù
        if (queryDto.IsEvenSemester.HasValue)
        {
            query = query.Where(d => d.IsEven.HasValue &&
                ((queryDto.IsEvenSemester.Value && d.IsEven.Value % 2 == 0) ||
                 (!queryDto.IsEvenSemester.Value && d.IsEven.Value % 2 == 1)));
        }

        // 5. ?ùùùùùùùù ùù ùùùùùùùù (Degree Levels)
        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));
        }

        // ?ùùùùùùùù ùùùùùùùù ùùùùùùù ùù ùùùùùùùù
        var totalCount = await query.CountAsync();

        // 6. ùùùùùùùùùù ùù ùùùù ùù
        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(d => d.NameAddDisciplines),
            2 => query.OrderByDescending(d => d.NameAddDisciplines),
            3 => query.OrderBy(d => d.CodeAddDisciplines),
            4 => query.OrderByDescending(d => d.CodeAddDisciplines),
            _ => query.OrderBy(d => d.IdAddDisciplines)
        };

        // 7. ùùùùùùùù (ùùùù: EF Core ùùùùùùùùùùù BindAddDisciplines.Count ùù ùùùùùùù SELECT COUNT(*))
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(d => new FullDisciplineDto
            {
                IdAddDisciplines = d.IdAddDisciplines ?? 0,
                NameAddDisciplines = d.NameAddDisciplines ?? "",
                CodeAddDisciplines = d.CodeAddDisciplines ?? "",
                FacultyId = d.FacultyId ?? 0,
                FacultyAbbreviation = d.Faculty != null ? d.Faculty.Abbreviation ?? "" : "",
                MaxCountPeople = d.MaxCountPeople,
                MinCourse = d.MinCourse,
                MaxCourse = d.MaxCourse,
                IsEven = d.IsEven.HasValue ? (sbyte?)d.IsEven.Value : null,
                DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegreec ?? "" : "",
                CountOfPeople = d.BindAddDisciplines.Count, // ?ùùùù ùùùùùù Include!
                IsAvailable = false // ùùù ùùùù-ùùùùù ùù ùùùù ùùùùùùùù ùùùùùùùùùùù, ùùù ùùùùù ùùùùùùù true
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
                s.BindAddDisciplines.Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindAddDisciplines = b.IdBindAddDisciplines ?? 0,
                    IdAddDisciplines = b.AddDisciplinesId ?? 0,
                    NameAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines ?? "" : "",
                    CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeAddDisciplines ?? "" : "",
                    Semestr = b.Semestr ?? 0,
                    InProcess = (sbyte)(b.InProcess ?? 0)
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

        if (queryDto.Groups != null && queryDto.Groups.Any())
            query = query.Where(s => queryDto.Groups.Contains(s.GroupId));

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
            s.BindAddDisciplines
                .Where(b => queryDto.IsNew == 0 || periodStart == null)
                .Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindAddDisciplines = b.IdBindAddDisciplines ?? 0,
                    IdAddDisciplines = b.AddDisciplinesId ?? 0,
                    NameAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines ?? "" : "",
                    CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeAddDisciplines ?? "" : "",
                    Semestr = b.Semestr ?? 0,
                    InProcess = (sbyte)(b.InProcess ?? 0)
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
            .GroupBy(n => new { Level = n.DegreeLevelId!.Value, IsFaculty = n.IsFaculty!.Value })
            .ToDictionaryAsync(g => (g.Key.Level, (sbyte)g.Key.IsFaculty), g => g.First().Count.GetValueOrDefault());
    }

    public async Task<List<DisciplineStatusProjection>> GetDisciplinesStatusDataAsync(GetDisciplinesWithStatusQueryDto queryDto)
    {
        var query = _context.AddDisciplines.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d => EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") || EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => d.FacultyId.HasValue && queryDto.Faculties.Contains(d.FacultyId.Value));

        if (queryDto.IsFaculty.HasValue)
            query = query.Where(d => d.IsFaculty == queryDto.IsFaculty.Value);

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));

        return await query.Select(d => new DisciplineStatusProjection(
            d.IdAddDisciplines ?? 0,
            d.NameAddDisciplines ?? "",
            d.AddDetail != null ? d.AddDetail.Teachers : null,
            d.AddDetail != null && d.AddDetail.Department != null ? d.AddDetail.Department.NameDepartment : null,
            d.MinCountPeople,
            d.MaxCountPeople,
            (sbyte)(d.IsForseChange ?? 0),
            d.Type != null ? d.Type.TypeName ?? "" : "",
            d.DegreeLevelId,
            (sbyte)(d.IsFaculty ?? 0),
            d.FacultyId ?? 0,
            d.Faculty != null ? d.Faculty.Abbreviation : null,
            d.BindAddDisciplines.Select(b => b.CreatedAt).ToList()
        )).ToListAsync();
    }

    public async Task<Dictionary<int, BindAddDiscipline>> GetBindsWithDetailsAsync(List<int> bindIds)
    {
        return await _context.BindAddDisciplines
            .Include(b => b.Student)
            .Include(b => b.AddDisciplines)
            .Where(b => b.IdBindAddDisciplines.HasValue && bindIds.Contains(b.IdBindAddDisciplines.Value))
            .ToDictionaryAsync(b => b.IdBindAddDisciplines!.Value);
    }

    public async Task<BindAddDisciplineDto?> GetBindDtoAsync(int id)
    {
        return await _context.BindAddDisciplines
            .AsNoTracking()
            .Where(b => b.IdBindAddDisciplines == id)
            .Select(b => new BindAddDisciplineDto
            {
                IdBindAddDisciplines = b.IdBindAddDisciplines ?? 0,
                StudentId = b.StudentId ?? 0,
                StudentFullName = b.Student != null ? b.Student.NameStudent ?? "" : "",
                AddDisciplinesId = b.AddDisciplinesId ?? 0,
                AddDisciplineName = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines ?? "" : "",
                Semestr = b.Semestr ?? 0,
                Loans = b.Loans ?? 0,
                InProcess = (b.InProcess ?? 0) == 1
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
                s.BindAddDisciplines.Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindAddDisciplines = b.IdBindAddDisciplines ?? 0,
                    IdAddDisciplines = b.AddDisciplinesId ?? 0,
                    NameAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines ?? "" : "",
                    CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeAddDisciplines ?? "" : "",
                    Semestr = b.Semestr ?? 0,
                    InProcess = (sbyte)(b.InProcess ?? 0)
                }).ToList()
            )).FirstOrDefaultAsync();
    }

    public async Task<BindAddDiscipline?> GetBindByStudentAndDisciplineAsync(int studentId, int disciplineId)
    {
        return await _context.BindAddDisciplines
            .Include(b => b.Student)
            .Include(b => b.AddDisciplines)
            .FirstOrDefaultAsync(b => b.StudentId == studentId && b.AddDisciplinesId == disciplineId);
    }

    public async Task<bool> ExistsBindAsync(int studentId, int disciplineId) =>
        await _context.BindAddDisciplines.AnyAsync(b => b.StudentId == studentId && b.AddDisciplinesId == disciplineId);

    public async Task<bool> ExistsStudentAsync(int studentId) =>
        await _context.Students.AnyAsync(s => s.IdStudent == studentId);

    public async Task<bool> ExistsDisciplineAsync(int disciplineId) =>
        await _context.AddDisciplines.AnyAsync(d => d.IdAddDisciplines == disciplineId);

    public async Task AddBindAsync(BindAddDiscipline bind) =>
        await _context.BindAddDisciplines.AddAsync(bind);

    public async Task<int> DeleteBindAsync(int id) =>
        await _context.BindAddDisciplines.Where(b => b.IdBindAddDisciplines == id).ExecuteDeleteAsync();

    public async Task<AddDiscipline?> GetDisciplineEntityAsync(int id) =>
        await _context.AddDisciplines.FindAsync(id);

    public void RemoveBind(BindAddDiscipline bind) => _context.BindAddDisciplines.Remove(bind);

    public void AddNotification(Notification notification) => _context.Notifications.Add(notification);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}