using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IEducationalProgramService
{
    Task<List<EducationalProgramFilterDto>> GetEducationalProgramsForFilterAsync(string? search);

    Task<PaginatedResponseDto<EducationalProgramDto>> GetEducationalProgramsAsync(EducationalProgramListQueryDto queryDto);

    Task<EducationalProgramDto?> GetEducationalProgramAsync(Guid id);

    Task<PaginatedResponseDto<ProgramStudentDto>> GetStudentsPagedAsync(Guid programId, ProgramStudentQueryDto queryDto);

    Task<List<ProgramDisciplinesBySemesterDto>> GetMainDisciplinesGroupedBySemesterAsync(Guid programId);

    Task<EducationalProgramDto> CreateEducationalProgramAsync(CreateEducationalProgramDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateEducationalProgramAsync(
        Guid id,
        UpdateEducationalProgramDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteEducationalProgramAsync(Guid id);
}

