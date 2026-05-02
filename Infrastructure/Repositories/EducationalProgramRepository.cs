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
            4 => query.OrderBy(ep => ep.StudentsAmount),
            5 => query.OrderByDescending(ep => ep.StudentsAmount),
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
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegreec ?? "" : "",
                SpecialityCode = ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : "",
                Speciality = ep.Speciality != null ? ep.Speciality.Name ?? "" : "",
                StudentsAmount = (uint)(ep.StudentsAmount ?? 0),
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
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegreec ?? "" : "",
                SpecialityCode = ep.Speciality != null && ep.Speciality.Code.HasValue ? ep.Speciality.Code.Value.ToString() : "",
                Speciality = ep.Speciality != null ? ep.Speciality.Name ?? "" : "",
                StudentsAmount = (uint)(ep.StudentsAmount ?? 0),
                StudentsCount = ep.Students.Count(),
                DisciplinesCount = ep.MainDisciplines.Count()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EducationalProgram?> GetEntityByIdAsync(int id) =>
        await _context.EducationalPrograms.FindAsync(id);

    public async Task<bool> ExistsAsync(int id) =>
        await _context.EducationalPrograms.AnyAsync(e => e.IdEducationalProgram == id);

    public async Task AddAsync(EducationalProgram program) =>
        await _context.EducationalPrograms.AddAsync(program);

    public async Task<int> DeleteAsync(int id) =>
        await _context.EducationalPrograms.Where(ep => ep.IdEducationalProgram == id).ExecuteDeleteAsync();

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}