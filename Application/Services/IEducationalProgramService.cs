using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IEducationalProgramService
{
    Task<PaginatedResponseDto<EducationalProgramDto>> GetEducationalProgramsAsync(EducationalProgramListQueryDto queryDto);

    Task<EducationalProgramDto?> GetEducationalProgramAsync(int id);

    Task<EducationalProgramDto> CreateEducationalProgramAsync(CreateEducationalProgramDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateEducationalProgramAsync(
        int id,
        UpdateEducationalProgramDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteEducationalProgramAsync(int id);
}

