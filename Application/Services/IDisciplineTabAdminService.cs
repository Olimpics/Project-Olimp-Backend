using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IDisciplineTabAdminService
{
    Task<PaginatedResponseDto<FullDisciplineDto>> GetAllDisciplinesAsync(GetAllDisciplinesAdminQueryDto queryDto);
    Task<PaginatedResponseDto<StudentWithDisciplineChoicesDto>> GetStudentsWithDisciplineChoicesAsync(GetStudentsWithDisciplineChoicesQueryDto query);

    Task<UpdateChoiceResponseDto> UpdateChoiceAsync(ConfirmOrRejectChoiceDto[] items);

    Task<PaginatedResponseDto<AdminDisciplineListItemDto>> GetDisciplinesWithStatusAsync(GetDisciplinesWithStatusQueryDto query);

    Task<UpdateDisciplineStatusResponseDto?> UpdateDisciplineStatusAsync(UpdateDisciplineStatusDto dto);

    Task<BindAddDisciplineDto?> GetBindAsync(int id);

    Task<StudentWithDisciplineChoicesDto?> GetStudentWithChoicesAsync(int studentId);

    Task<(int? bindId, string? error)> CreateBindAsync(AddDisciplineBindDto dto);

    Task<bool> DeleteBindAsync(int id);

    Task<PaginatedResponseDto<AdminStudentByAddDisciplineDto>> GetStudentsByAddDisciplineAsync(GetStudentsByAddDisciplineQueryDto query);

    Task<PaginatedResponseDto<AdminStudentByMainDisciplineDto>> GetStudentsByMainDisciplineAsync(GetStudentsByMainDisciplineQueryDto query);

    /// <summary>Students (id and name) who still lack required add-discipline selections after the last completed choice period for the faculty.</summary>
    Task<List<StudentIdNameDto>> GetStudentsIncompleteAfterChoicePeriodAsync(int facultyId);
}
