using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IDisciplineTabService
{
    Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto query);

    Task<DisciplineTabResponseDto?> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto);
    Task<(Guid? bindId, string? error)> SelectiveDisciplineBindAsync(SelectiveDisciplineBindDto dto);

    Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(Guid id);
    Task<List<SimilarDisciplineGroupDto>> GetSimilarDisciplinesAsync(Guid disciplineId);
    Task<bool> RemoveDisciplineFromSimilarityGroupAsync(Guid disciplineId, Guid groupId);
}
