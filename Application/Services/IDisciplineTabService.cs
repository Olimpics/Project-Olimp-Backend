using OlimpBack.Application.DTO;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IDisciplineTabService
{
    Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto query);

    Task<DisciplineTabResponseDto?> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto);
    Task<(int? bindId, string? error)> SelectiveDisciplineBindAsync(SelectiveDisciplineBindDto dto);

    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(int id);

    Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateSelectiveDisciplineWithDetailsDto dto);

    Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(int id, UpdateSelectiveDisciplineWithDetailsDto dto);

}
