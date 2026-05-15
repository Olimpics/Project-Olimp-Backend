using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            .Where(b => b.SelectiveDisciplineId == query.DisciplineId);

        if (query.FacultyId.HasValue && query.FacultyId.Value != Guid.Empty)
        {
            baseQuery = baseQuery.Where(b => b.Student.Group.EducationalProgram.Speciality.Department.FacultyId == query.FacultyId.Value);
        }

        if (query.GroupId.HasValue && query.GroupId.Value != Guid.Empty)
        {
            baseQuery = baseQuery.Where(b => b.Student.GroupId == query.GroupId.Value);
        }

        if (query.DepartmentId.HasValue && query.DepartmentId.Value != Guid.Empty)
        {
            baseQuery = baseQuery.Where(b => b.Student.Group.EducationalProgram.Speciality.Department.IdDepartment == query.DepartmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            baseQuery = baseQuery.Where(b =>
                EF.Functions.Like(b.Student.NameStudent.ToLower(), $"%{search}%") ||
                EF.Functions.Like(b.Student.Group.GroupCode.ToLower(), $"%{search}%") ||
                (b.Student.Group.EducationalProgram.Speciality.Department != null &&
                 EF.Functions.Like(b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment.ToLower(), $"%{search}%")) ||
                EF.Functions.Like(b.Student.Group.Degree.NameEducationalDegree.ToLower(), $"%{search}%"));
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
            5 => baseQuery.OrderByDescending(b => b.Student.Group != null
                ? b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment
                : string.Empty),
            6 => baseQuery.OrderBy(b => b.Student.Course),
            7 => baseQuery.OrderByDescending(b => b.Student.Course),
            8 => baseQuery.OrderBy(b => b.Student.Group.Degree.NameEducationalDegree),
            9 => baseQuery.OrderByDescending(b => b.Student.Group.Degree.NameEducationalDegree),
            10 => baseQuery.OrderBy(b => b.Student.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty),
            11 => baseQuery.OrderByDescending(b => b.Student.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty),
            _ => baseQuery.OrderBy(b => b.Student.NameStudent)
        };

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(b => new AdminStudentBySelectiveDisciplineDto
            {
                StudentId = b.StudentId,
                StudentName = b.Student.NameStudent ?? "",
                GroupId = b.Student.GroupId,
                GroupCode = b.Student.Group.GroupCode ?? "",
                DepartmentName = b.Student.Group.EducationalProgram.Speciality.Department != null
                    ? b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment ?? ""
                    : string.Empty,
                Year = b.Student.Course,
                EducationLevel = b.Student.Group.Degree != null ? b.Student.Group.Degree.NameEducationalDegree ?? "" : "",
                IsShort = b.Student.IsShort ? true : false,
                Faculty = b.Student.Group.EducationalProgram.Speciality.Department.Faculty != null ? b.Student.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty ?? "" : ""
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<(int totalCount, List<AdminStudentByMainDisciplineDto> items)> GetStudentsByMainDisciplineAsync(GetStudentsByMainDisciplineQueryDto query)
    {
        var baseQuery = _context.MainGrades
            .AsNoTracking()
            .Where(g => g.MainDisciplinesId == query.DisciplineId);

        if (query.FacultyId.HasValue && query.FacultyId.Value != Guid.Empty)
        {
            baseQuery = baseQuery.Where(g => g.Student.Group.EducationalProgram.Speciality.Department.FacultyId == query.FacultyId.Value);
        }

        if (query.GroupId.HasValue && query.GroupId.Value != Guid.Empty)
        {
            baseQuery = baseQuery.Where(g => g.Student.GroupId == query.GroupId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            baseQuery = baseQuery.Where(g =>
                EF.Functions.Like(g.Student.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty.ToLower(), $"%{search}%") ||
                EF.Functions.Like(g.Student.Group.GroupCode.ToLower(), $"%{search}%"));
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
            .Select(g => new AdminStudentByMainDisciplineDto
            {
                StudentId = g.StudentId,
                StudentName = g.Student.NameStudent ?? "",
                FacultyId = g.Student.Group.EducationalProgram.Speciality.Department.FacultyId,
                FacultyName = g.Student.Group.EducationalProgram.Speciality.Department.Faculty != null ? g.Student.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty ?? "" : "",
                GroupId = g.Student.GroupId,
                GroupCode = g.Student.Group.GroupCode ?? "",
                Course = g.Student.Course
            })
            .ToListAsync();

        return (totalCount, page);
    }
}
