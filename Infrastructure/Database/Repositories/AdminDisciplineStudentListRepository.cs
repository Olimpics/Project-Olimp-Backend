using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IAdminDisciplineStudentListRepository
{
    Task<(int totalCount, List<AdminStudentBySelectiveDisciplineDto> items)> GetStudentsBySelectiveDisciplineAsync(GetStudentsBySelectiveDisciplineQueryDto query);
    Task<(int totalCount, List<AdminStudentByMainDisciplineDto> items)> GetStudentsByMainDisciplineAsync(GetStudentsByMainDisciplineQueryDto query);
}

public class AdminDisciplineStudentListRepository : IAdminDisciplineStudentListRepository
{
    private readonly AppDbContext _context;

    public AdminDisciplineStudentListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int totalCount, List<AdminStudentBySelectiveDisciplineDto> items)> GetStudentsBySelectiveDisciplineAsync(GetStudentsBySelectiveDisciplineQueryDto query)
    {
        var baseQuery = _context.BindSelectiveDisciplines
            .AsNoTracking()
            .Where(b => b.SelectiveDisciplinesId == query.DisciplineId);

        if (query.FacultyId is > 0)
        {
            baseQuery = baseQuery.Where(b => b.Student.FacultyId == query.FacultyId);
        }

        if (query.GroupId is > 0)
        {
            baseQuery = baseQuery.Where(b => b.Student.GroupId == query.GroupId);
        }

        if (query.DepartmentId is > 0)
        {
            baseQuery = baseQuery.Where(b => b.Student.Group.EducationalProgram.Speciality.Department.IdDepartment == query.DepartmentId);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            baseQuery = baseQuery.Where(b =>
                EF.Functions.Like(b.Student.NameStudent, $"%{search}%") ||
                EF.Functions.Like(b.Student.Group.GroupCode, $"%{search}%") ||
                (b.Student.Group.EducationalProgram.Speciality.Department != null &&
                 EF.Functions.Like(b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment, $"%{search}%")) ||
                EF.Functions.Like(b.Student.EducationalDegree.NameEducationalDegree, $"%{search}%"));
        }

        var totalCount = await baseQuery.CountAsync();

        baseQuery = query.SortOrder switch
        {
            0 => baseQuery.OrderBy(b => b.Student.NameStudent),
            1 => baseQuery.OrderByDescending(b => b.Student.NameStudent),
            2 => baseQuery.OrderBy(b => b.Student.Group.GroupCode),
            3 => baseQuery.OrderByDescending(b => b.Student.Group.GroupCode),
            4 => baseQuery.OrderBy(b => b.Student.Group != null
                ? b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment
                : string.Empty),
            5 => baseQuery.OrderByDescending(b => b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment != null
                ? b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment
                : string.Empty),
            6 => baseQuery.OrderBy(b => b.Student.Course),
            7 => baseQuery.OrderByDescending(b => b.Student.Course),
            8 => baseQuery.OrderBy(b => b.Student.EducationalDegree.NameEducationalDegree),
            9 => baseQuery.OrderByDescending(b => b.Student.EducationalDegree.NameEducationalDegree),
            10 => baseQuery.OrderBy(b => b.Student.Faculty.NameFaculty),
            11 => baseQuery.OrderByDescending(b => b.Student.Faculty.NameFaculty),
            _ => baseQuery.OrderBy(b => b.Student.NameStudent)
        };

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(b => new AdminStudentBySelectiveDisciplineDto
            {
                StudentId = b.StudentId ?? 0,
                StudentName = b.Student.NameStudent,
                GroupId = b.Student.GroupId,
                GroupCode = b.Student.Group.GroupCode,
                DepartmentName = b.Student.Group.EducationalProgram.Speciality.Department != null
                    ? b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment
                    : string.Empty,
                Year = b.Student.Course,
                EducationLevel = b.Student.EducationalDegree.NameEducationalDegree,
                IsShort = (sbyte)b.Student.IsShort,
                Faculty = b.Student.Faculty.NameFaculty
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

        var page = await grouped
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var items = page.Select(g => new AdminStudentByMainDisciplineDto
        {
            StudentId = g.StudentId ?? 0,
            StudentName = g.Student?.NameStudent ?? "",
            FacultyId = g.Student?.FacultyId ?? 0,
            FacultyName = g.Student?.Faculty?.NameFaculty ?? "",
            GroupId = g.Student?.GroupId ?? 0,
            GroupCode = g.Student?.Group?.GroupCode ?? "",
            Course = g.Student?.Course ?? 0
        }).ToList();

        return (totalCount, items);
    }
}

