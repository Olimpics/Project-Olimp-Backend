using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IStudentService
{
    Task<PaginatedResponseDto<StudentForCatalogDto>> GetStudentsAsync(StudentQueryDto queryDto);

    Task<StudentDto?> GetStudentAsync(int id);

    Task<IReadOnlyList<StudentDto>> CreateStudentsAsync(IReadOnlyList<CreateStudentDto> dtos);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateStudentAsync(int id, UpdateStudentDto dto);
}

