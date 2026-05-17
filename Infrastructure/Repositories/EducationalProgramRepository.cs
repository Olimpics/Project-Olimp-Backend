using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IEducationalProgramRepository
{
    Task<List<EducationalProgramFilterDto>> GetForFilterAsync(string? search);
    Task<(int TotalCount, List<EducationalProgramDto> Items)> GetPagedAsync(EducationalProgramListQueryDto queryDto);
    Task<EducationalProgramDto?> GetDtoByIdAsync(Guid id);
    Task<EducationalProgram?> GetEntityByIdAsync(Guid id);
    Task<(int TotalCount, List<ProgramStudentDto> Items)> GetStudentsPagedAsync(Guid programId, ProgramStudentQueryDto queryDto);
    Task<List<ProgramDisciplinesBySemesterDto>> GetMainDisciplinesGroupedBySemesterAsync(Guid programId);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(EducationalProgram program);
    Task<int> DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public class EducationalProgramRepository : IEducationalProgramRepository
{
    private readonly AppDbContext _context;

    public EducationalProgramRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationalProgramFilterDto>> GetForFilterAsync(string? search)
    {
        var query = _context.EducationalPrograms.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.Trim().ToLower();
            query = query.Where(ep =>
                EF.Functions.Like((ep.NameEducationalProgram ?? "").ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like((ep.Speciality != null && ep.Speciality.Code.Length > 0 ? ep.Speciality.Code : "").ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like((ep.Speciality != null && ep.Speciality.Name != null ? ep.Speciality.Name : "").ToLower(), $"%{lowerSearch}%"));
        }

        return await query
            .OrderBy(ep => ep.NameEducationalProgram)
            .Select(ep => new EducationalProgramFilterDto
            {
                Id = ep.IdEducationalProgram,
                Name = ep.NameEducationalProgram ?? ""
            })
            .ToListAsync();
    }

    public async Task<(int TotalCount, List<EducationalProgramDto> Items)> GetPagedAsync(EducationalProgramListQueryDto queryDto)
    {
        var query = _context.EducationalPrograms.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(ep =>
                EF.Functions.Like((ep.NameEducationalProgram ?? "").ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like((ep.Speciality != null && ep.Speciality.Code.Length > 0 ? ep.Speciality.Code : "").ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(ep => ep.DegreeId != Guid.Empty && queryDto.DegreeLevelIds.Contains(ep.DegreeId));
        }

        var totalCount = await query.CountAsync();

        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(ep => ep.NameEducationalProgram),
            2 => query.OrderByDescending(ep => ep.NameEducationalProgram),
            3 => query.OrderByDescending(ep => ep.Speciality != null && ep.Speciality.Code.Length > 0 ? ep.Speciality.Code : ""),
            _ => query.OrderBy(ep => ep.Speciality != null && ep.Speciality.Code.Length > 0 ? ep.Speciality.Code : "")
        };

        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram ?? "",
                DegreeId = ep.DegreeId,
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegree ?? "" : "",
                SpecialityCode = ep.Speciality != null && ep.Speciality.Code.Length > 0 ? ep.Speciality.Code : "",
                Speciality = ep.Speciality != null ? ep.Speciality.Name ?? "" : "",
                StudentsCount = ep.StudentGroups != null
                    ? ep.StudentGroups.SelectMany(g => g.Students).Count()
                    : 0,
                DisciplinesCount = ep.MainDisciplines.Count()
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<EducationalProgramDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.EducationalPrograms
            .AsNoTracking()
            .Where(ep => ep.IdEducationalProgram == id)
            .Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram ?? "",
                DegreeId = ep.DegreeId,
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegree ?? "" : "",
                SpecialityCode = ep.Speciality != null && ep.Speciality.Code != null ? ep.Speciality.Code : "",
                Speciality = ep.Speciality != null ? ep.Speciality.Name ?? "" : "",
                StudentsCount = ep.StudentGroups != null
                    ? ep.StudentGroups.SelectMany(g => g.Students).Count()
                    : 0,
                DisciplinesCount = ep.MainDisciplines.Count()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EducationalProgram?> GetEntityByIdAsync(Guid id) =>
        await _context.EducationalPrograms.FindAsync(id);

    public async Task<(int TotalCount, List<ProgramStudentDto> Items)> GetStudentsPagedAsync(Guid programId, ProgramStudentQueryDto queryDto)
    {
        var query = _context.Students
            .AsNoTracking()
            .Where(s => s.Group.EducationalProgramId != null && s.Group.EducationalProgramId == programId);

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s => EF.Functions.Like(s.FirstName.ToLower(), $"%{lowerSearch}%") ||
                                     (s.SecondName != null && EF.Functions.Like(s.SecondName.ToLower(), $"%{lowerSearch}%")) ||
                                     (s.ThirdName != null && EF.Functions.Like(s.ThirdName.ToLower(), $"%{lowerSearch}%")));
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryDto.SortBy))
        {
            query = queryDto.SortBy.ToLower() switch
            {
                "group" => queryDto.IsDescending ? query.OrderByDescending(s => s.Group.GroupCode) : query.OrderBy(s => s.Group.GroupCode),
                "isshort" => queryDto.IsDescending ? query.OrderByDescending(s => s.Group.IsAccelerated) : query.OrderBy(s => s.Group.IsAccelerated),
                "status" => queryDto.IsDescending ? query.OrderByDescending(s => s.EducationStatus.NameEducationStatus) : query.OrderBy(s => s.EducationStatus.NameEducationStatus),
                "educationstart" => queryDto.IsDescending ? query.OrderByDescending(s => s.EducationStart) : query.OrderBy(s => s.EducationStart),
                "name" => queryDto.IsDescending 
                    ? query.OrderByDescending(s => s.SecondName).ThenByDescending(s => s.FirstName).ThenByDescending(s => s.ThirdName) 
                    : query.OrderBy(s => s.SecondName).ThenBy(s => s.FirstName).ThenBy(s => s.ThirdName),
                _ => query.OrderBy(s => s.SecondName).ThenBy(s => s.FirstName).ThenBy(s => s.ThirdName)
            };
        }
        else
        {
            query = query.OrderBy(s => s.SecondName).ThenBy(s => s.FirstName).ThenBy(s => s.ThirdName);
        }

        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(s => new ProgramStudentDto
            {
                IdStudent = s.IdStudent,
                FirstName = s.FirstName ?? "",
                SecondName = s.SecondName ?? "",
                ThirdName = s.ThirdName ?? "",
                GroupName = s.Group.GroupCode ?? "",
                IsShort = s.Group != null ? s.Group.IsAccelerated : false,
                Status = s.EducationStatus.NameEducationStatus ?? "",
                EducationStart = s.EducationStart
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<List<ProgramDisciplinesBySemesterDto>> GetMainDisciplinesGroupedBySemesterAsync(Guid programId)
    {
        var disciplines = await _context.MainDisciplines
            .AsNoTracking()
            .Where(d => d.EducationalProgramId == programId)
            .Select(d => new
            {
                d.IdMainDisciplines,
                d.CodeMainDisciplines,
                d.NameMainDisciplines,
                d.FormControl,
                Loans = d.Loans ?? 0,
                Hours = d.Hours ?? 0,
                Semester = d.Semestr
            })
            .ToListAsync();

        return disciplines
            .GroupBy(d => d.Semester)
            .Select(g => new ProgramDisciplinesBySemesterDto
            {
                Semester = g.Key,
                Disciplines = g.Select(d => new ProgramMainDisciplineDto
                {
                    Id = d.IdMainDisciplines,
                    Code = d.CodeMainDisciplines ?? "",
                    Name = d.NameMainDisciplines ?? "",
                    FormControl = d.FormControl,
                    Loans = d.Loans,
                    Hours = d.Hours
                }).ToList()
            })
            .OrderBy(g => g.Semester)
            .ToList();
    }

    public async Task<bool> ExistsAsync(Guid id) =>
        await _context.EducationalPrograms.AnyAsync(e => e.IdEducationalProgram == id);

    public async Task AddAsync(EducationalProgram program) =>
        await _context.EducationalPrograms.AddAsync(program);

    public async Task<int> DeleteAsync(Guid id) =>
        await _context.EducationalPrograms.Where(ep => ep.IdEducationalProgram == id).ExecuteDeleteAsync();

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}