using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IEducationalProgramService
{
    Task<List<EducationalProgramFilterDto>> GetEducationalProgramsForFilterAsync(string? search);

    Task<PaginatedResponseDto<EducationalProgramDto>> GetEducationalProgramsAsync(EducationalProgramListQueryDto queryDto);

    Task<EducationalProgramDto?> GetEducationalProgramAsync(int id);

    Task<PaginatedResponseDto<ProgramStudentDto>> GetStudentsPagedAsync(int programId, ProgramStudentQueryDto queryDto);

    Task<List<ProgramDisciplinesBySemesterDto>> GetMainDisciplinesGroupedBySemesterAsync(int programId);

    Task<EducationalProgramDto> CreateEducationalProgramAsync(CreateEducationalProgramDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateEducationalProgramAsync(
        int id,
        UpdateEducationalProgramDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteEducationalProgramAsync(int id);
}

