using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IFacultyService
{
    Task<IEnumerable<FacultyDto>> GetFacultiesAsync();

    Task<FacultyDto?> GetFacultyAsync(Guid id);

    Task<FacultyDto> CreateFacultyAsync(FacultyCreateDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateFacultyAsync(Guid id, FacultyDto dto);
}

