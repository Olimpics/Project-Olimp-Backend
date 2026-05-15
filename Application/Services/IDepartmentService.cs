using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IDepartmentService
{
    Task<PaginatedResponseDto<DepartmentDto>> GetDepartmentsAsync(DepartmentQueryDto queryDto);

    Task<DepartmentDto?> GetDepartmentAsync(Guid id);

    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteDepartmentAsync(Guid id);
}

