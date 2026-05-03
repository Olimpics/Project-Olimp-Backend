using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IEducationalProgramRepository
{
    Task<List<EducationalProgramFilterDto>> GetForFilterAsync(string? search);
    Task<(int TotalCount, List<EducationalProgramDto> Items)> GetPagedAsync(EducationalProgramListQueryDto queryDto);
    Task<EducationalProgramDto?> GetDtoByIdAsync(int id);
    Task<EducationalProgram?> GetEntityByIdAsync(int id);
    Task<(int TotalCount, List<ProgramStudentDto> Items)> GetStudentsPagedAsync(int programId, ProgramStudentQueryDto queryDto);
    Task<List<ProgramDisciplinesBySemesterDto>> GetMainDisciplinesGroupedBySemesterAsync(int programId);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(EducationalProgram program);
    Task<int> DeleteAsync(int id);
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
                EF.Functions.Like((ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : "").ToLower(), $"%{lowerSearch}%") ||
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
                EF.Functions.Like((ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : "").ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(ep => ep.DegreeId.HasValue && queryDto.DegreeLevelIds.Contains(ep.DegreeId.Value));
        }

        var totalCount = await query.CountAsync();

        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(ep => ep.NameEducationalProgram),
            2 => query.OrderByDescending(ep => ep.NameEducationalProgram),
            3 => query.OrderByDescending(ep => ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : ""),
            _ => query.OrderBy(ep => ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : "")
        };

        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram ?? "",
                DegreeId = ep.DegreeId ?? 0,
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegree ?? "" : "",
                SpecialityCode = ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : "",
                Speciality = ep.Speciality != null ? ep.Speciality.Name ?? "" : "",
                StudentsCount = ep.Students.Count(),
                DisciplinesCount = ep.MainDisciplines.Count()
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<EducationalProgramDto?> GetDtoByIdAsync(int id)
    {
        return await _context.EducationalPrograms
            .AsNoTracking()
            .Where(ep => ep.IdEducationalProgram == id)
            .Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram ?? "",
                DegreeId = ep.DegreeId ?? 0,
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegree ?? "" : "",
                SpecialityCode = ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : "",
                Speciality = ep.Speciality != null ? ep.Speciality.Name ?? "" : "",
                StudentsCount = ep.Students.Count(),
                DisciplinesCount = ep.MainDisciplines.Count()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EducationalProgram?> GetEntityByIdAsync(int id) =>
        await _context.EducationalPrograms.FindAsync(id);

    public async Task<(int TotalCount, List<ProgramStudentDto> Items)> GetStudentsPagedAsync(int programId, ProgramStudentQueryDto queryDto)
    {
        var query = _context.Students
            .AsNoTracking()
            .Where(s => s.EducationalProgramId == programId);

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s => EF.Functions.Like((s.NameStudent ?? "").ToLower(), $"%{lowerSearch}%"));
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryDto.SortBy))
        {
            query = queryDto.SortBy.ToLower() switch
            {
                "group" => queryDto.IsDescending ? query.OrderByDescending(s => s.Group.GroupCode) : query.OrderBy(s => s.Group.GroupCode),
                "isshort" => queryDto.IsDescending ? query.OrderByDescending(s => s.IsShort) : query.OrderBy(s => s.IsShort),
                "status" => queryDto.IsDescending ? query.OrderByDescending(s => s.EducationStatus.NameEducationStatus) : query.OrderBy(s => s.EducationStatus.NameEducationStatus),
                "educationstart" => queryDto.IsDescending ? query.OrderByDescending(s => s.EducationStart) : query.OrderBy(s => s.EducationStart),
                "name" => queryDto.IsDescending ? query.OrderByDescending(s => s.NameStudent) : query.OrderBy(s => s.NameStudent),
                _ => query.OrderBy(s => s.NameStudent)
            };
        }
        else
        {
            query = query.OrderBy(s => s.NameStudent);
        }

        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(s => new ProgramStudentDto
            {
                IdStudent = s.IdStudent,
                NameStudent = s.NameStudent ?? "",
                GroupName = s.Group.GroupCode ?? "",
                IsShort = s.IsShort != 0,
                Status = s.EducationStatus.NameEducationStatus ?? "",
                EducationStart = s.EducationStart
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<List<ProgramDisciplinesBySemesterDto>> GetMainDisciplinesGroupedBySemesterAsync(int programId)
    {
        var disciplines = await _context.MainDisciplines
            .AsNoTracking()
            .Where(d => d.EducationalProgramId == programId)
            .Select(d => new
            {
                d.IdBindMainDisciplines,
                d.CodeMainDisciplines,
                d.NameBindMainDisciplines,
                d.FormControl,
                Loans = d.Loans ?? 0,
                Hours = d.Hours ?? 0,
                Semester = d.Semestr ?? 0
            })
            .ToListAsync();

        return disciplines
            .GroupBy(d => d.Semester)
            .Select(g => new ProgramDisciplinesBySemesterDto
            {
                Semester = g.Key,
                Disciplines = g.Select(d => new ProgramMainDisciplineDto
                {
                    Id = d.IdBindMainDisciplines,
                    Code = d.CodeMainDisciplines ?? "",
                    Name = d.NameBindMainDisciplines ?? "",
                    FormControl = d.FormControl,
                    Loans = d.Loans,
                    Hours = d.Hours
                }).ToList()
            })
            .OrderBy(g => g.Semester)
            .ToList();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.EducationalPrograms.AnyAsync(e => e.IdEducationalProgram == id);

    public async Task AddAsync(EducationalProgram program) =>
        await _context.EducationalPrograms.AddAsync(program);

    public async Task<int> DeleteAsync(int id) =>
        await _context.EducationalPrograms.Where(ep => ep.IdEducationalProgram == id).ExecuteDeleteAsync();

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}