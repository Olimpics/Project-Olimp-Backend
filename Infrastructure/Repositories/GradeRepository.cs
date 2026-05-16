using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IGradeRepository
{
    Task<(int TotalCount, List<GradeStudentDto> Items)> GetMainDisciplineStudentsPagedAsync(GradeQueryDto queryDto);
    Task<(int TotalCount, List<GradeStudentDto> Items)> GetSelectiveDisciplineStudentsPagedAsync(GradeQueryDto queryDto);
    Task<BindMainDiscipline?> GetMainBindAsync(Guid id);
    Task<BindSelectiveDiscipline?> GetSelectiveBindAsync(Guid id);
    Task<List<InstructorDisciplineDto>> GetMainDisciplinesByInstructorAsync(Guid adminId, Guid catalogYearId, bool isEvenSemester);
    Task<List<InstructorDisciplineDto>> GetSelectiveDisciplinesByInstructorAsync(Guid adminId, Guid catalogYearId, bool isEvenSemester);
    Task<AcademicPeriodDto> GetAcademicPeriodAsync(DateTime date);
    Task SaveChangesAsync();
}

public class GradeRepository : IGradeRepository
{
    private readonly AppDbContext _context;

    public GradeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalCount, List<GradeStudentDto> Items)> GetMainDisciplineStudentsPagedAsync(GradeQueryDto queryDto)
    {
        var parity = queryDto.IsEvenSemester ? 0 : 1;
        var query = _context.BindMainDisciplines.AsNoTracking()
            .Where(b => b.MainDisciplinesId == queryDto.DisciplineId && b.YearId == queryDto.CatalogYearId && b.Semestr % 2 == parity);

        var projection = query.Select(b => new GradeStudentDto
        {
            IdBind = b.IdBindMainDisciplines,
            FullName = b.Student != null ? b.Student.NameStudent ?? "" : "",
            GroupName = b.Student != null && b.Student.Group != null ? b.Student.Group.GroupCode ?? "" : "",
            DepartmentName = b.Student != null && b.Student.Group != null && b.Student.Group.EducationalProgram != null && b.Student.Group.EducationalProgram.Speciality != null && b.Student.Group.EducationalProgram.Speciality.Department != null ? b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment ?? "" : "",
            FacultyName = b.Student != null && b.Student.Group != null && b.Student.Group.EducationalProgram != null && b.Student.Group.EducationalProgram.Speciality != null && b.Student.Group.EducationalProgram.Speciality.Department != null && b.Student.Group.EducationalProgram.Speciality.Department.Faculty != null ? b.Student.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation ?? "" : "",
            Score = b.Grade >= 0 ? b.Grade : (int?)null
        });

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var s = queryDto.Search.ToLower();
            projection = projection.Where(p =>
                p.FullName.ToLower().Contains(s) ||
                p.GroupName.ToLower().Contains(s) ||
                p.DepartmentName.ToLower().Contains(s) ||
                p.FacultyName.ToLower().Contains(s));
        }

        var totalCount = await projection.CountAsync();

        projection = queryDto.SortBy?.ToLower() switch
        {
            "fullname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.FullName) : projection.OrderBy(p => p.FullName),
            "groupname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.GroupName) : projection.OrderBy(p => p.GroupName),
            "departmentname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.DepartmentName) : projection.OrderBy(p => p.DepartmentName),
            "facultyname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.FacultyName) : projection.OrderBy(p => p.FacultyName),
            "score" => queryDto.SortDescending ? projection.OrderByDescending(p => p.Score) : projection.OrderBy(p => p.Score),
            _ => projection.OrderBy(p => p.FullName)
        };

        var items = await projection
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<(int TotalCount, List<GradeStudentDto> Items)> GetSelectiveDisciplineStudentsPagedAsync(GradeQueryDto queryDto)
    {
        var parity = queryDto.IsEvenSemester ? 0 : 1;
        var query = _context.BindSelectiveDisciplines.AsNoTracking()
            .Where(b => b.SelectiveDisciplineId == queryDto.DisciplineId && b.YearId == queryDto.CatalogYearId && b.Semestr % 2 == parity);

        var projection = query.Select(b => new GradeStudentDto
        {
            IdBind = b.IdBindSelectiveDisciplines,
            FullName = b.Student != null ? b.Student.NameStudent ?? "" : "",
            GroupName = b.Student != null && b.Student.Group != null ? b.Student.Group.GroupCode ?? "" : "",
            DepartmentName = b.Student != null && b.Student.Group != null && b.Student.Group.EducationalProgram.Speciality.Department != null ? b.Student.Group.EducationalProgram.Speciality.Department.NameDepartment ?? "" : "",
            FacultyName = b.Student != null && b.Student.Group != null && b.Student.Group.EducationalProgram.Speciality.Department.Faculty != null ? b.Student.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation ?? "" : "",
            Score = b.Grade
        });

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var s = queryDto.Search.ToLower();
            projection = projection.Where(p =>
                p.FullName.ToLower().Contains(s) ||
                p.GroupName.ToLower().Contains(s) ||
                p.DepartmentName.ToLower().Contains(s) ||
                p.FacultyName.ToLower().Contains(s));
        }

        var totalCount = await projection.CountAsync();

        projection = queryDto.SortBy?.ToLower() switch
        {
            "fullname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.FullName) : projection.OrderBy(p => p.FullName),
            "groupname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.GroupName) : projection.OrderBy(p => p.GroupName),
            "departmentname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.DepartmentName) : projection.OrderBy(p => p.DepartmentName),
            "facultyname" => queryDto.SortDescending ? projection.OrderByDescending(p => p.FacultyName) : projection.OrderBy(p => p.FacultyName),
            "score" => queryDto.SortDescending ? projection.OrderByDescending(p => p.Score) : projection.OrderBy(p => p.Score),
            _ => projection.OrderBy(p => p.FullName)
        };

        var items = await projection
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<BindMainDiscipline?> GetMainBindAsync(Guid id)
    {
        return await _context.BindMainDisciplines.FindAsync(id);
    }

    public async Task<BindSelectiveDiscipline?> GetSelectiveBindAsync(Guid id)
    {
        return await _context.BindSelectiveDisciplines.FindAsync(id);
    }

    public async Task<List<InstructorDisciplineDto>> GetMainDisciplinesByInstructorAsync(Guid adminId, Guid catalogYearId, bool isEvenSemester)
    {
        var parity = isEvenSemester ? 0 : 1;

        return await _context.BindTeacherMains
            .Where(btm => btm.AdminId == adminId)
            .Where(btm => _context.BindMainDisciplines.Any(bmd =>
                bmd.MainDisciplinesId == btm.MainDisciplinesId &&
                bmd.YearId == catalogYearId &&
                bmd.Semestr % 2 == parity))
            .Select(btm => new InstructorDisciplineDto
            {
                Id = btm.MainDisciplinesId,
                Title = btm.MainDisciplines != null ? btm.MainDisciplines.NameBindMainDisciplines ?? "" : ""
            })
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<InstructorDisciplineDto>> GetSelectiveDisciplinesByInstructorAsync(Guid adminId, Guid catalogYearId, bool isEvenSemester)
    {
        var parity = isEvenSemester ? 0 : 1;

        return await _context.BindTeachersSelectives
            .Where(bts => bts.AdminId == adminId)
            .Where(bts => _context.BindSelectiveDisciplines.Any(bsd =>
                bsd.SelectiveDisciplineId == bts.SelectiveDisciplinesId &&
                bsd.YearId == catalogYearId &&
                bsd.Semestr % 2 == parity))
            .Select(bts => new InstructorDisciplineDto
            {
                Id = bts.SelectiveDisciplinesId,
                Title = bts.SelectiveDisciplines != null ? bts.SelectiveDisciplines.NameSelectiveDisciplines ?? "" : ""
            })
            .Distinct()
            .ToListAsync();
    }

    public async Task<AcademicPeriodDto> GetAcademicPeriodAsync(DateTime date)
    {
        var semStarts = await _context.SemestersStarts.ToListAsync();
        var sem1Start = semStarts.FirstOrDefault(s => s.StartDate.Month == 9)?.StartDate ?? new DateOnly(1900, 9, 1);
        var sem2Start = semStarts.FirstOrDefault(s => s.StartDate.Month == 2)?.StartDate ?? new DateOnly(1900, 2, 1);

        int month = date.Month;
        int day = date.Day;
        var dateVal = month * 100 + day;
        var s1Val = sem1Start.Month * 100 + sem1Start.Day;
        var s2Val = sem2Start.Month * 100 + sem2Start.Day;

        bool isSem2;
        if (s2Val < s1Val)
        {
            isSem2 = dateVal >= s2Val && dateVal < s1Val;
        }
        else
        {
            isSem2 = dateVal >= s2Val || dateVal < s1Val;
        }

        int semester = isSem2 ? 2 : 1;
        int yearStart;

        if (semester == 2)
        {
            yearStart = date.Year - 1;
        }
        else
        {
            yearStart = date.Year;
        }

        var catalogYear = await _context.CatalogYears.FirstOrDefaultAsync(cy => cy.YearStart == yearStart);

        return new AcademicPeriodDto
        {
            CatalogYearId = catalogYear?.IdCatalog ?? Guid.Empty,
            Semester = semester
        };
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
