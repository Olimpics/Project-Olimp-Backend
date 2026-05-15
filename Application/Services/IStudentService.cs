using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IStudentService
{
    Task<PaginatedResponseDto<StudentForCatalogDto>> GetStudentsAsync(StudentQueryDto queryDto);

    Task<StudentDto?> GetStudentAsync(Guid id);

    Task<IReadOnlyList<StudentDto>> CreateStudentsAsync(IReadOnlyList<CreateStudentDto> dtos);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateStudentAsync(Guid id, UpdateStudentDto dto);
}
