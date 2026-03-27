using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IAdminDisciplineStudentListRepository
{
    Task<(int totalCount, List<AdminStudentByAddDisciplineDto> items)> GetStudentsByAddDisciplineAsync(GetStudentsByAddDisciplineQueryDto query);
    Task<(int totalCount, List<AdminStudentByMainDisciplineDto> items)> GetStudentsByMainDisciplineAsync(GetStudentsByMainDisciplineQueryDto query);
}

public class AdminDisciplineStudentListRepository : IAdminDisciplineStudentListRepository
{
    private readonly AppDbContext _context;

    public AdminDisciplineStudentListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int totalCount, List<AdminStudentByAddDisciplineDto> items)> GetStudentsByAddDisciplineAsync(GetStudentsByAddDisciplineQueryDto query)
    {
        var baseQuery = _context.BindAddDisciplines
            .AsNoTracking()
            .Include(b => b.Student)
                .ThenInclude(s => s.EducationalDegree)
            .Include(b => b.Student)
                .ThenInclude(s => s.Faculty)
            .Include(b => b.Student)
                .ThenInclude(s => s.Group)
                    .ThenInclude(g => g.Department)
            .Where(b => b.AddDisciplinesId == query.DisciplineId);

        if (query.FacultyId.HasValue && query.FacultyId.Value > 0)
        {
            baseQuery = baseQuery.Where(b => b.Student.FacultyId == query.FacultyId.Value);
        }

        if (query.GroupId.HasValue && query.GroupId.Value > 0)
        {
            baseQuery = baseQuery.Where(b => b.Student.GroupId == query.GroupId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            baseQuery = baseQuery.Where(b =>
                b.Student.NameStudent.Contains(search) ||
                b.Student.Group.GroupCode.Contains(search) ||
                (b.Student.Group.Department != null && b.Student.Group.Department.NameDepartment.Contains(search)) ||
                b.Student.EducationalDegree.NameEducationalDegreec.Contains(search));
        }

        baseQuery = query.SortOrder switch
        {
            1 => baseQuery.OrderByDescending(b => b.Student.NameStudent),
            2 => baseQuery.OrderBy(b => b.Student.Group.GroupCode),
            3 => baseQuery.OrderByDescending(b => b.Student.Group.GroupCode),
            4 => baseQuery.OrderBy(b => b.Student.Group.Department != null ? b.Student.Group.Department.NameDepartment : string.Empty),
            5 => baseQuery.OrderByDescending(b => b.Student.Group.Department != null ? b.Student.Group.Department.NameDepartment : string.Empty),
            6 => baseQuery.OrderBy(b => b.Student.Course),
            7 => baseQuery.OrderByDescending(b => b.Student.Course),
            8 => baseQuery.OrderBy(b => b.Student.EducationalDegree.NameEducationalDegreec),
            9 => baseQuery.OrderByDescending(b => b.Student.EducationalDegree.NameEducationalDegreec),
            10 => baseQuery.OrderBy(b => b.Student.IsShort),
            11 => baseQuery.OrderByDescending(b => b.Student.IsShort),
            12 => baseQuery.OrderBy(b => b.CreatedAt),
            13 => baseQuery.OrderByDescending(b => b.CreatedAt),
            _ => baseQuery.OrderBy(b => b.Student.NameStudent)
        };

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(b => new AdminStudentByAddDisciplineDto
            {
                StudentId = b.StudentId,
                StudentName = b.Student.NameStudent,
                GroupId = b.Student.GroupId,
                GroupCode = b.Student.Group.GroupCode,
                DepartmentName = b.Student.Group.Department != null ? b.Student.Group.Department.NameDepartment : string.Empty,
                Year = b.Student.Course,
                EducationLevel = b.Student.EducationalDegree.NameEducationalDegreec,
                IsShort = b.Student.IsShort,
                ChoiceDate = b.CreatedAt
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<(int totalCount, List<AdminStudentByMainDisciplineDto> items)> GetStudentsByMainDisciplineAsync(GetStudentsByMainDisciplineQueryDto query)
    {
        var baseQuery = _context.MainGrades
            .AsNoTracking()
            .Include(g => g.Student)
                .ThenInclude(s => s.Faculty)
            .Include(g => g.Student)
                .ThenInclude(s => s.Group)
            .Where(g => g.MainDisciplinesId == query.DisciplineId);

        if (query.FacultyId.HasValue && query.FacultyId.Value > 0)
        {
            baseQuery = baseQuery.Where(g => g.Student.FacultyId == query.FacultyId.Value);
        }

        if (query.GroupId.HasValue && query.GroupId.Value > 0)
        {
            baseQuery = baseQuery.Where(g => g.Student.GroupId == query.GroupId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            baseQuery = baseQuery.Where(g =>
                g.Student.Faculty.NameFaculty.Contains(search) ||
                g.Student.Group.GroupCode.Contains(search));
        }

        // We want distinct students per discipline
        var grouped = baseQuery
            .GroupBy(g => g.StudentId)
            .Select(g => g.First());

        grouped = query.SortOrder switch
        {
            1 => grouped.OrderByDescending(g => g.Student.NameStudent),
            2 => grouped.OrderBy(g => g.Student.Group.GroupCode),
            3 => grouped.OrderByDescending(g => g.Student.Group.GroupCode),
            _ => grouped.OrderBy(g => g.Student.NameStudent)
        };

        var totalCount = await grouped.CountAsync();

        var items = await grouped
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(g => new AdminStudentByMainDisciplineDto
            {
                StudentId = g.StudentId,
                StudentName = g.Student.NameStudent,
                FacultyId = g.Student.FacultyId,
                FacultyName = g.Student.Faculty.NameFaculty,
                GroupId = g.Student.GroupId,
                GroupCode = g.Student.Group.GroupCode,
                Course = g.Student.Course
            })
            .ToListAsync();

        return (totalCount, items);
    }
}

