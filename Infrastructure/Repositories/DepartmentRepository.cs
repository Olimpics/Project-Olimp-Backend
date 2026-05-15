using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDepartmentRepository
{
    Task<(int TotalCount, List<DepartmentDto> Items)> GetPagedAsync(DepartmentQueryDto queryDto);
    Task<DepartmentDto?> GetDtoByIdAsync(Guid id);
    Task<Department?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(Department department);
    Task<int> DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalCount, List<DepartmentDto> Items)> GetPagedAsync(DepartmentQueryDto queryDto)
    {
        var query = _context.Departments.AsNoTracking().AsQueryable();

        if (queryDto.FacultyIds != null && queryDto.FacultyIds.Any())
            query = query.Where(d => d.FacultyId != Guid.Empty && queryDto.FacultyIds.Contains(d.FacultyId));

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            if (Guid.TryParse(lowerSearch, out Guid searchId))
                query = query.Where(d => d.IdDepartment == searchId);
            else
                query = query.Where(d =>
                    EF.Functions.Like(d.NameDepartment.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(d.Abbreviation.ToLower(), $"%{lowerSearch}%"));
        }

        var totalCount = await query.CountAsync();

        query = queryDto.SortOrder switch
        {
            1 => query.OrderByDescending(d => d.Abbreviation),
            2 => query.OrderBy(d => d.Faculty.Abbreviation),
            3 => query.OrderByDescending(d => d.Faculty.Abbreviation),
            _ => query.OrderBy(d => d.Abbreviation)
        };

        // Блискавична проекція без Include!
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(d => new DepartmentDto
            {
                IdDepartment = d.IdDepartment,
                FacultyId = d.FacultyId,
                NameDepartment = d.NameDepartment ?? "",
                Abbreviation = d.Abbreviation ?? "",
                FacultyName = d.Faculty != null ? d.Faculty.NameFaculty ?? "" : ""
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<DepartmentDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.IdDepartment == id)
            .Select(d => new DepartmentDto
            {
                IdDepartment = d.IdDepartment,
                FacultyId = d.FacultyId,
                NameDepartment = d.NameDepartment ?? "",
                Abbreviation = d.Abbreviation ?? "",
                FacultyName = d.Faculty != null ? d.Faculty.NameFaculty ?? "" : ""
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Department?> GetEntityByIdAsync(Guid id) =>
        await _context.Departments.FindAsync(id);

    public async Task<bool> ExistsAsync(Guid id) =>
        await _context.Departments.AnyAsync(d => d.IdDepartment == id);

    public async Task AddAsync(Department department) =>
        await _context.Departments.AddAsync(department);

    public async Task<int> DeleteAsync(Guid id) =>
        await _context.Departments.Where(d => d.IdDepartment == id).ExecuteDeleteAsync();

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}