using OlimpBack.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public interface ISimilarityService
{
    Task GenerateKeysForAllAsync();
    Task FormGroupsAsync();
    Task ProcessNewDisciplinesAsync();
    Task<List<SimilarGroupBlockDto>> GetSimilarByDisciplineIdAsync(Guid disciplineId);
    Task RemoveDisciplineFromGroupAsync(Guid groupId, Guid disciplineId);
}

public interface IEpSimilarityService
{
    Task GenerateKeysForAllAsync();
    Task FormGroupsAsync();
    Task ProcessNewProgramsAsync();
    Task<List<SimilarEpGroupBlockDto>> GetSimilarByEpIdAsync(Guid epId);
    Task RemoveEpFromGroupAsync(Guid groupId, Guid epId);
}

public class SimilarGroupBlockDto
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public List<DisciplineSimpleDto> Disciplines { get; set; } = new();
}

public class DisciplineSimpleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class SimilarEpGroupBlockDto
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public List<EpSimpleDto> Programs { get; set; } = new();
}

public class EpSimpleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
