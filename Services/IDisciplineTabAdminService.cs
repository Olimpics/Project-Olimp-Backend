using OlimpBack.DTO;

namespace OlimpBack.Services;

public interface IDisciplineTabAdminService
{
    Task<object?> GetStudentsWithDisciplineChoicesAsync(GetStudentsWithDisciplineChoicesQueryDto query);

    Task<object> UpdateChoiceAsync(ConfirmOrRejectChoiceDto[] items);

    Task<object?> GetDisciplinesWithStatusAsync(GetDisciplinesWithStatusQueryDto query);

    Task<object?> UpdateDisciplineStatusAsync(UpdateDisciplineStatusDto dto);

    Task<BindAddDisciplineDto?> GetBindAsync(int id);

    Task<StudentWithDisciplineChoicesDto?> GetStudentWithChoicesAsync(int studentId);

    Task<(int? bindId, string? error)> CreateBindAsync(AdminCreateBindDto dto);

    Task<bool> DeleteBindAsync(int id);
}
