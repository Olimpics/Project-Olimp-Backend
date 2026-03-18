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
}
