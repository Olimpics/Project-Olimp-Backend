using Microsoft.Extensions.Logging;
using OlimpBack.Infrastructure.Repositories;
using OlimpBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public class SimilarityService : ISimilarityService
{
    private readonly ISimilarityRepository _repository;
    private readonly ILogger<SimilarityService> _logger;

    public SimilarityService(ISimilarityRepository repository, ILogger<SimilarityService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task GenerateKeysForAllAsync()
    {
        var disciplines = await _repository.GetAllDisciplinesWithDetailsAsync();
        foreach (var d in disciplines)
        {
            d.Keys = ExtractKeywords(d);
        }
        await _repository.SaveChangesAsync();
    }

    public async Task FormGroupsAsync()
    {
        await _repository.ClearAllGroupsAsync();
        var disciplines = await _repository.GetAllDisciplinesWithDetailsAsync();

        foreach (var central in disciplines)
        {
            var group = new GroupSimilarSelective
            {
                IdGroup = Guid.NewGuid(),
                CentrallId = central.IdSelectiveDisciplines,
                GroupName = central.NameSelectiveDisciplines
            };
            await _repository.AddGroupAsync(group);

            // Compare with all (including self)
            foreach (var other in disciplines)
            {
                if (IsSimilar(central, other))
                {
                    await _repository.AddBindingAsync(new BindSimilarSelectiveInGroup
                    {
                        IdBind = Guid.NewGuid(),
                        GroupId = group.IdGroup,
                        SelectiveId = other.IdSelectiveDisciplines
                    });
                }
            }
        }
        await _repository.SaveChangesAsync();
    }

    public async Task ProcessNewDisciplinesAsync()
    {
        var disciplines = await _repository.GetAllDisciplinesWithDetailsAsync();
        var groups = await _repository.GetAllGroupsWithDisciplinesAsync();

        // Disciplines that are NOT in any group
        var existingBinds = groups.SelectMany(g => g.BindSimilarSelectiveInGroups).Select(b => b.SelectiveId).ToHashSet();
        var newDisciplines = disciplines.Where(d => !existingBinds.Contains(d.IdSelectiveDisciplines)).ToList();

        if (!newDisciplines.Any()) return;

        foreach (var d in newDisciplines)
        {
            d.Keys = ExtractKeywords(d); // Ensure keys exist
            
            foreach (var group in groups)
            {
                // Find central discipline for this group
                var central = disciplines.FirstOrDefault(x => x.IdSelectiveDisciplines == group.CentrallId);
                if (central != null && IsSimilar(central, d))
                {
                    await _repository.AddBindingAsync(new BindSimilarSelectiveInGroup
                    {
                        IdBind = Guid.NewGuid(),
                        GroupId = group.IdGroup,
                        SelectiveId = d.IdSelectiveDisciplines
                    });
                }
            }
        }
        await _repository.SaveChangesAsync();
    }

    public async Task<List<SimilarGroupBlockDto>> GetSimilarByDisciplineIdAsync(Guid disciplineId)
    {
        var groups = await _repository.GetGroupsBySelectiveIdAsync(disciplineId);
        return groups.Select(g => new SimilarGroupBlockDto
        {
            GroupId = g.IdGroup,
            GroupName = g.GroupName,
            Disciplines = g.BindSimilarSelectiveInGroups.Select(b => new DisciplineSimpleDto
            {
                Id = b.Selective.IdSelectiveDisciplines,
                Name = b.Selective.NameSelectiveDisciplines
            }).ToList()
        }).ToList();
    }

    public async Task RemoveDisciplineFromGroupAsync(Guid groupId, Guid disciplineId)
    {
        await _repository.RemoveFromGroupAsync(groupId, disciplineId);
        await _repository.SaveChangesAsync();
    }

    private List<string> ExtractKeywords(SelectiveDiscipline d)
    {
        var text = $"{d.NameSelectiveDisciplines} {d.CodeSelectiveDisciplines} {d.SelectiveDetail?.WhyInterestingDetermination} {d.SelectiveDetail?.ResultEducation} {d.SelectiveDetail?.Provision}";
        return Tokenize(text);
    }

    private List<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new List<string>();
        
        // Remove HTML tags if any
        text = Regex.Replace(text, "<.*?>", string.Empty);
        
        var words = Regex.Matches(text.ToLower(), @"\w{4,}") // Words with at least 4 chars
            .Select(m => m.Value)
            .Where(w => !StopWords.Contains(w))
            .Distinct()
            .Take(15) // Limit to 15 keywords
            .ToList();

        return words;
    }

    private bool IsSimilar(SelectiveDiscipline a, SelectiveDiscipline b)
    {
        if (a.IdSelectiveDisciplines == b.IdSelectiveDisciplines) return true;

        // Name match (more than one word)
        var nameWordsA = a.NameSelectiveDisciplines.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 3).ToHashSet();
        var nameWordsB = b.NameSelectiveDisciplines.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 3).ToHashSet();
        int nameOverlap = nameWordsA.Intersect(nameWordsB).Count();

        // Key match
        int keyOverlap = 0;
        if (a.Keys != null && b.Keys != null)
        {
            keyOverlap = a.Keys.Intersect(b.Keys).Count();
        }

        return (nameOverlap + keyOverlap) > 1;
    }

    private static readonly HashSet<string> StopWords = new() 
    { 
        "також", "його", "який", "яка", "яке", "які", "цього", "тому", "через", "після", "перед", "вона", "воно", "вони",
        "буде", "було", "були", "мати", "може", "можуть", "свою", "свої", "свого", "яких", "яким", "якою", "якої", "цьому", "цієї"
        // Add more common Ukrainian stop words as needed
    };
}
