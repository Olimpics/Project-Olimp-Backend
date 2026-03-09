using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;

namespace OlimpBack.Services;

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;

    public DepartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponseDto<DepartmentDto>> GetDepartmentsAsync(DepartmentQueryDto queryDto)
    {
        var query = _context.Departments
            .Include(d => d.Faculty)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.FacultyIds))
        {
            var facultyIdList = queryDto.FacultyIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (facultyIdList.Any())
            {
                query = query.Where(d => facultyIdList.Contains(d.FacultyId));
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();

            if (int.TryParse(queryDto.Search.Trim(), out int searchId))
            {
                query = query.Where(d => d.IdDepartment == searchId);
            }
            else
            {
                query = query.Where(d =>
                    EF.Functions.Like(d.NameDepartment.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(d.Abbreviation.ToLower(), $"%{lowerSearch}%"));
            }
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        var departments = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        departments = queryDto.SortOrder switch
        {
            1 => departments.OrderByDescending(d => d.Abbreviation).ToList(),
            2 => departments.OrderBy(d => d.Faculty.Abbreviation).ToList(),
            3 => departments.OrderByDescending(d => d.Faculty.Abbreviation).ToList(),
            _ => departments.OrderBy(d => d.Abbreviation).ToList()
        };

        var items = departments.Select(d => new DepartmentDto
        {
            IdDepartment = d.IdDepartment,
            FacultyId = d.FacultyId,
            NameDepartment = d.NameDepartment,
            Abbreviation = d.Abbreviation,
            FacultyName = d.Faculty.NameFaculty
        }).ToList();

        return new PaginatedResponseDto<DepartmentDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items
        };
    }

    public async Task<DepartmentDto?> GetDepartmentAsync(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Faculty)
            .FirstOrDefaultAsync(d => d.IdDepartment == id);

        if (department == null)
            return null;

        return new DepartmentDto
        {
            IdDepartment = department.IdDepartment,
            FacultyId = department.FacultyId,
            NameDepartment = department.NameDepartment,
            Abbreviation = department.Abbreviation,
            FacultyName = department.Faculty.NameFaculty
        };
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto)
    {
        var department = new Models.Department
        {
            FacultyId = dto.FacultyId,
            NameDepartment = dto.NameDepartment,
            Abbreviation = dto.Abbreviation
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        await _context.Entry(department).Reference(d => d.Faculty).LoadAsync();

        return new DepartmentDto
        {
            IdDepartment = department.IdDepartment,
            FacultyId = department.FacultyId,
            NameDepartment = department.NameDepartment,
            Abbreviation = department.Abbreviation,
            FacultyName = department.Faculty.NameFaculty
        };
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto)
    {
        if (id != dto.IdDepartment)
            return (false, StatusCodes.Status400BadRequest, "Route id does not match body id.");

        var department = await _context.Departments.FindAsync(id);
         if (department == null)
            return (false, StatusCodes.Status404NotFound, "Department not found");

        department.FacultyId = dto.FacultyId;
        department.NameDepartment = dto.NameDepartment;
        department.Abbreviation = dto.Abbreviation;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Departments.AnyAsync(d => d.IdDepartment == id);
            if (!exists)
                return (false, StatusCodes.Status404NotFound, "Department not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteDepartmentAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return (false, StatusCodes.Status404NotFound, "Department not found");

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return (true, StatusCodes.Status204NoContent, null);
    }
}

