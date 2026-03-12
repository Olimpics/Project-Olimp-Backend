using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IFacultyService
{
    Task<IEnumerable<FacultyDto>> GetFacultiesAsync();

    Task<FacultyDto?> GetFacultyAsync(int id);

    Task<FacultyDto> CreateFacultyAsync(FacultyCreateDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateFacultyAsync(int id, FacultyDto dto);
}

