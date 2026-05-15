using System;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IDisciplineTabService
{
    Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto query);

    Task<DisciplineTabResponseDto?> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto);
    Task<(Guid? bindId, string? error)> SelectiveDisciplineBindAsync(SelectiveDisciplineBindDto dto);

    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(Guid id);

    Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateSelectiveDisciplineWithDetailsDto dto);

    Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(Guid id, UpdateSelectiveDisciplineWithDetailsDto dto);

    Task<(bool success, string? error)> UpdateDisciplineStatusAsync(Guid id, Guid statusId);
}
