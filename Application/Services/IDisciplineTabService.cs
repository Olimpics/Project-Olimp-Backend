using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IDisciplineTabService
{
    Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto query);


    Task<(int? bindId, string? error)> AddDisciplineBindAsync(AddDisciplineBindDto dto);

    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(int id);

    Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateAddDisciplineWithDetailsDto dto);

    Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(int id, UpdateAddDisciplineWithDetailsDto dto);

}
