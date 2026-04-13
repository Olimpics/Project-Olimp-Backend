using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDisciplineTabAdminRepository
{
    Task<(int TotalCount, List<FullDisciplineDto> Items)> GetAllDisciplinesPagedAsync(GetAllDisciplinesAdminQueryDto queryDto);
    Task<DateTime?> GetLastPeriodStartDateAsync(int facultyId);
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

        // 1. Фільтрація по пошуку
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        // 2. Фільтрація по факультетах
        if (queryDto.Faculties != null && queryDto.Faculties.Any())
        {
            query = query.Where(d => queryDto.Faculties.Contains(d.FacultyId));
        }

        // 3. Фільтрація по курсах
        if (queryDto.Courses != null && queryDto.Courses.Any())
        {
            query = query.Where(d =>
                (!d.MinCourse.HasValue || queryDto.Courses.Contains(d.MinCourse.Value)) &&
                (!d.MaxCourse.HasValue || queryDto.Courses.Contains(d.MaxCourse.Value)));
        }

        // 4. Фільтрація по семестру
        if (queryDto.IsEvenSemester.HasValue)
        {
            query = query.Where(d => d.IsEven.HasValue &&
                ((queryDto.IsEvenSemester.Value && d.IsEven.Value % 2 == 0) ||
                 (!queryDto.IsEvenSemester.Value && d.IsEven.Value % 2 == 1)));
        }

        // 5. Фільтрація по ступенях (Degree Levels)
        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));
        }

        // Підрахунок загальної кількості до пагінації
        var totalCount = await query.CountAsync();

        // 6. Сортування на рівні БД
        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(d => d.NameAddDisciplines),
            2 => query.OrderByDescending(d => d.NameAddDisciplines),
            3 => query.OrderBy(d => d.CodeAddDisciplines),
            4 => query.OrderByDescending(d => d.CodeAddDisciplines),
            _ => query.OrderBy(d => d.IdAddDisciplines)
        };

        // 7. Проекція (Магія: EF Core перетворить BindAddDisciplines.Count на підзапит SELECT COUNT(*))
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(d => new FullDisciplineDto
            {
                IdAddDisciplines = d.IdAddDisciplines,
                NameAddDisciplines = d.NameAddDisciplines,
                CodeAddDisciplines = d.CodeAddDisciplines,
                FacultyId = d.FacultyId,
                FacultyAbbreviation = d.Faculty != null ? d.Faculty.Abbreviation : "",
                MaxCountPeople = d.MaxCountPeople,
                MinCourse = d.MinCourse,
                MaxCourse = d.MaxCourse,
                IsEven = d.IsEven,
                DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegreec : "",
                CountOfPeople = d.BindAddDisciplines.Count, // Ніяких важких Include!
                IsAvailable = false // Для адмін-панелі це поле зазвичай неактуальне, або можна ставити true
            })
            .ToListAsync();

        return (totalCount, items);
    }
    public async Task<DateTime?> GetLastPeriodStartDateAsync(int facultyId)
    {
        return await _context.DisciplineChoicePeriods
            .Where(p => p.FacultyId == facultyId)
            .OrderByDescending(p => p.StartDate)
            .Select(p => p.StartDate)
            .FirstOrDefaultAsync();
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
                .Where(b => queryDto.IsNew == 0 || periodStart == null || b.CreatedAt >= periodStart)
                .Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindAddDisciplines = b.IdBindAddDisciplines,
                    IdAddDisciplines = b.AddDisciplinesId,
                    NameAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines : "",
                    CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeAddDisciplines : "",
                    Semestr = b.Semestr,
                    InProcess = b.InProcess
                }).ToList()
        )).ToListAsync();
    }

    public async Task<Dictionary<int, DateTime>> GetLastPeriodsByFacultyAsync()
    {
        return await _context.DisciplineChoicePeriods
            .Where(p => p.FacultyId != null)
            .GroupBy(p => p.FacultyId!.Value)
            .Select(g => new { FacultyId = g.Key, StartDate = g.Max(p => p.StartDate) })
            .ToDictionaryAsync(x => x.FacultyId, x => x.StartDate);
    }

    public async Task<Dictionary<(int Level, sbyte IsFaculty), int>> GetNormativesLookupAsync()
    {
        return await _context.Normatives
            .Where(n => n.DegreeLevelId != null)
            .GroupBy(n => new { Level = n.DegreeLevelId!.Value, n.IsFaculty })
            .ToDictionaryAsync(g => (g.Key.Level, g.Key.IsFaculty), g => g.First().Count);
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
            query = query.Where(d => queryDto.Faculties.Contains(d.FacultyId));

        if (queryDto.IsFaculty.HasValue)
            query = query.Where(d => d.IsFaculty == queryDto.IsFaculty.Value);

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));

        return await query.Select(d => new DisciplineStatusProjection(
            d.IdAddDisciplines,
            d.NameAddDisciplines,
            d.AddDetail != null ? d.AddDetail.Teachers : null,
            d.AddDetail != null && d.AddDetail.Department != null ? d.AddDetail.Department.NameDepartment : null,
            d.MinCountPeople,
            d.MaxCountPeople,
            d.IsForseChange,
            d.Type != null ? d.Type.TypeName : "",
            d.DegreeLevelId,
            d.IsFaculty,
            d.FacultyId,
            d.Faculty != null ? d.Faculty.Abbreviation : null,
            d.BindAddDisciplines.Select(b => b.CreatedAt).ToList()
        )).ToListAsync();
    }

    public async Task<Dictionary<int, BindAddDiscipline>> GetBindsWithDetailsAsync(List<int> bindIds)
    {
        return await _context.BindAddDisciplines
            .Include(b => b.Student)
            .Include(b => b.AddDisciplines)
            .Where(b => bindIds.Contains(b.IdBindAddDisciplines))
            .ToDictionaryAsync(b => b.IdBindAddDisciplines);
    }

    public async Task<BindAddDisciplineDto?> GetBindDtoAsync(int id)
    {
        return await _context.BindAddDisciplines
            .AsNoTracking()
            .Where(b => b.IdBindAddDisciplines == id)
            .Select(b => new BindAddDisciplineDto
            {
                IdBindAddDisciplines = b.IdBindAddDisciplines,
                StudentId = b.StudentId,
                StudentFullName = b.Student != null ? b.Student.NameStudent : "",
                AddDisciplinesId = b.AddDisciplinesId,
                AddDisciplineName = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines : "",
                Semestr = b.Semestr,
                Loans = b.Loans,
                InProcess = b.InProcess == 1
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
                    IdBindAddDisciplines = b.IdBindAddDisciplines,
                    IdAddDisciplines = b.AddDisciplinesId,
                    NameAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines : "",
                    CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeAddDisciplines : "",
                    Semestr = b.Semestr,
                    InProcess = b.InProcess
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