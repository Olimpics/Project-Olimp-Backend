using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResponseDto<DepartmentDto>> GetDepartmentsAsync(DepartmentQueryDto queryDto)
    {
        var (totalCount, items) = await _repository.GetPagedAsync(queryDto);
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        return new PaginatedResponseDto<DepartmentDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items
        };
    }

    public async Task<DepartmentDto?> GetDepartmentAsync(int id) =>
        await _repository.GetDtoByIdAsync(id);

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            FacultyId = dto.FacultyId,
            NameDepartment = dto.NameDepartment,
            Abbreviation = dto.Abbreviation
        };

        await _repository.AddAsync(department);
        await _repository.SaveChangesAsync();

        // Повертаємо через GetDtoByIdAsync, щоб підтягнулася FacultyName
        return (await _repository.GetDtoByIdAsync(department.IdDepartment))!;
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto)
    {
        if (id != dto.IdDepartment)
            return (false, StatusCodes.Status400BadRequest, "Route id does not match body id.");

        var department = await _repository.GetEntityByIdAsync(id);
        if (department == null)
            return (false, StatusCodes.Status404NotFound, "Department not found");

        department.FacultyId = dto.FacultyId;
        department.NameDepartment = dto.NameDepartment;
        department.Abbreviation = dto.Abbreviation;

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Department not found");
            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteDepartmentAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (deleted == 0)
            return (false, StatusCodes.Status404NotFound, "Department not found");

        return (true, StatusCodes.Status204NoContent, null);
    }
}