using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IDisciplineTabAdminService
{
    Task<PaginatedResponseDto<FullDisciplineDto>> GetAllDisciplinesAsync(GetAllDisciplinesAdminQueryDto queryDto);
    Task<PaginatedResponseDto<StudentWithDisciplineChoicesDto>> GetStudentsWithDisciplineChoicesAsync(GetStudentsWithDisciplineChoicesQueryDto query);

    Task<UpdateChoiceResponseDto> UpdateChoiceAsync(ConfirmOrRejectChoiceDto[] items);

    Task<PaginatedResponseDto<AdminDisciplineListItemDto>> GetDisciplinesWithStatusAsync(GetDisciplinesWithStatusQueryDto query);

    Task<UpdateDisciplineStatusResponseDto?> UpdateDisciplineStatusAsync(UpdateDisciplineStatusDto dto);

    Task<BindSelectiveDisciplineDto?> GetBindAsync(Guid id);

    Task<StudentWithDisciplineChoicesDto?> GetStudentWithChoicesAsync(Guid studentId);

    Task<(Guid? bindId, string? error)> CreateBindAsync(SelectiveDisciplineBindDto dto);

    Task<bool> DeleteBindAsync(Guid id);

    Task<PaginatedResponseDto<AdminStudentBySelectiveDisciplineDto>> GetStudentsBySelectiveDisciplineAsync(GetStudentsBySelectiveDisciplineQueryDto query);

    Task<(bool success, string? errorMessage)> RepealChoiceAsync(Guid DisciplinId, Guid studentId);

    Task<PaginatedResponseDto<AdminStudentByMainDisciplineDto>> GetStudentsByMainDisciplineAsync(GetStudentsByMainDisciplineQueryDto query);

    /// <summary>Students (id and name) who still lack required add-discipline selections after the last completed choice period for the faculty.</summary>
    Task<List<StudentIdNameDto>> GetStudentsIncompleteAfterChoicePeriodAsync(Guid facultyId);
}
