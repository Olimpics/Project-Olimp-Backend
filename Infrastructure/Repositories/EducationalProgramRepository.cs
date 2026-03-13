using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IEducationalProgramRepository
{
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

    public async Task<(int TotalCount, List<EducationalProgramDto> Items)> GetPagedAsync(EducationalProgramListQueryDto queryDto)
    {
        var query = _context.EducationalPrograms.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(ep =>
                EF.Functions.Like(ep.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(ep.SpecialityCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(ep => queryDto.DegreeLevelIds.Contains(ep.DegreeId));
        }

        var totalCount = await query.CountAsync();

        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(ep => ep.NameEducationalProgram),
            2 => query.OrderByDescending(ep => ep.NameEducationalProgram),
            3 => query.OrderByDescending(ep => ep.SpecialityCode),
            4 => query.OrderBy(ep => ep.StudentsAmount),
            5 => query.OrderByDescending(ep => ep.StudentsAmount),
            _ => query.OrderBy(ep => ep.SpecialityCode)
        };

        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram,
                DegreeId = ep.DegreeId,
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegreec : "",
                SpecialityCode = ep.SpecialityCode,
                Speciality = ep.Speciality,
                StudentsAmount = ep.StudentsAmount,
                StudentsCount = ep.Students.Count(),
                DisciplinesCount = ep.BindMainDisciplines.Count()
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
                NameEducationalProgram = ep.NameEducationalProgram,
                DegreeId = ep.DegreeId,
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegreec : "",
                SpecialityCode = ep.SpecialityCode,
                Speciality = ep.Speciality,
                StudentsAmount = ep.StudentsAmount,
                StudentsCount = ep.Students.Count(),
                DisciplinesCount = ep.BindMainDisciplines.Count()
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