using System;
using System.Collections.Generic;
using System.Linq;
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
        return await _context.SelectiveDisciplines
            .Where(d => d.IdSelectiveDisciplines == id)
            .Select(d => new FullDisciplineWithDetailsDto
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
                Recomend = d.SelectiveDetail != null ? d.SelectiveDetail.Recommended : null,
                Prerequisites = d.SelectiveDetail != null ? d.SelectiveDetail.Prerequisites : null,
                Language = d.SelectiveDetail != null ? d.SelectiveDetail.Language : null,
                Provision = d.SelectiveDetail != null ? d.SelectiveDetail.Provision : null,
                WhyInterestingDetermination = d.SelectiveDetail != null ? d.SelectiveDetail.WhyInterestingDetermination : null,
                ResultEducation = d.SelectiveDetail != null ? d.SelectiveDetail.ResultEducation : null,
                UsingIrl = d.SelectiveDetail != null ? d.SelectiveDetail.UsingIrl : null,
                DisciplineTopics = d.SelectiveDetail != null && d.SelectiveDetail.DisciplineTopics != null ? string.Join(", ", d.SelectiveDetail.DisciplineTopics) : null,
                TypesOfTraining = d.SelectiveDetail != null ? d.SelectiveDetail.TypesOfTraining : "",
                TypeOfControl = d.TypeOfControl != null ? d.TypeOfControl.Type : "",
                CatalogId = d.CatalogId,
                ApprovalStatusId = d.ApprovalStatusId,
                TypeOfControlId = d.TypeOfControlId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SelectiveDiscipline?> GetDisciplineWithDetailEntityAsync(Guid id) =>
        await _context.SelectiveDisciplines.Include(d => d.SelectiveDetail).FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == id);

    public async Task<bool> DepartmentExistsAsync(Guid departmentId) =>
        await _context.Departments.AnyAsync(d => d.IdDepartment == departmentId);

    public async Task SelectiveDisciplineAsync(SelectiveDiscipline discipline) =>
        await _context.SelectiveDisciplines.AddAsync(discipline);

    public async Task AddBindAsync(BindSelectiveDiscipline bind) =>
        await _context.BindSelectiveDisciplines.AddAsync(bind);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
