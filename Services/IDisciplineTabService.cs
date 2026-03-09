using OlimpBack.DTO;

namespace OlimpBack.Services;

public interface IDisciplineTabService
{
    Task<object?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto query);


    Task<(int? bindId, string? error)> AddDisciplineBindAsync(AddDisciplineBindDto dto);

    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(int id);

    Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateAddDisciplineWithDetailsDto dto);

    Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(int id, UpdateAddDisciplineWithDetailsDto dto);

}
