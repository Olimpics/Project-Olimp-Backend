using Microsoft.Extensions.Logging;
using OlimpBack.Infrastructure.Repositories;
using OlimpBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public class EpSimilarityService : IEpSimilarityService
{
    private readonly IEpSimilarityRepository _repository;
    private readonly ILogger<EpSimilarityService> _logger;

    public EpSimilarityService(IEpSimilarityRepository repository, ILogger<EpSimilarityService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task GenerateKeysForAllAsync()
    {
        var programs = await _repository.GetAllProgramsAsync();
        foreach (var p in programs)
        {
            p.Keys = ExtractKeywords(p);
        }
        await _repository.SaveChangesAsync();
    }

    public async Task FormGroupsAsync()
    {
        await _repository.ClearAllGroupsAsync();
        var programs = await _repository.GetAllProgramsAsync();

        foreach (var central in programs)
        {
            var group = new GroupSimilarEducationalProgram
            {
                IdGroup = Guid.NewGuid(),
                CentralId = central.IdEducationalProgram,
                GroupName = central.NameEducationalProgram
            };
            await _repository.AddGroupAsync(group);

            foreach (var other in programs)
            {
                if (IsSimilar(central, other))
                {
                    await _repository.AddBindingAsync(new BindSimilaEducationalProgramInGroup
                    {
                        IdBind = Guid.NewGuid(),
                        GroupId = group.IdGroup,
                        EducationalProgramId = other.IdEducationalProgram
                    });
                }
            }
        }
        await _repository.SaveChangesAsync();
    }

    public async Task ProcessNewProgramsAsync()
    {
        var programs = await _repository.GetAllProgramsAsync();
        var groups = await _repository.GetAllGroupsWithProgramsAsync();

        var existingBinds = groups.SelectMany(g => g.BindSimilaEducationalProgramInGroups).Select(b => b.EducationalProgramId).ToHashSet();
        var newPrograms = programs.Where(p => !existingBinds.Contains(p.IdEducationalProgram)).ToList();

        if (!newPrograms.Any()) return;

        foreach (var p in newPrograms)
        {
            p.Keys = ExtractKeywords(p);
            
            foreach (var group in groups)
            {
                var central = programs.FirstOrDefault(x => x.IdEducationalProgram == group.CentralId);
                if (central != null && IsSimilar(central, p))
                {
                    await _repository.AddBindingAsync(new BindSimilaEducationalProgramInGroup
                    {
                        IdBind = Guid.NewGuid(),
                        GroupId = group.IdGroup,
                        EducationalProgramId = p.IdEducationalProgram
                    });
                }
            }
        }
        await _repository.SaveChangesAsync();
    }

    public async Task<List<SimilarEpGroupBlockDto>> GetSimilarByEpIdAsync(Guid epId)
    {
        var groups = await _repository.GetGroupsByEpIdAsync(epId);
        return groups.Select(g => new SimilarEpGroupBlockDto
        {
            GroupId = g.IdGroup,
            GroupName = g.GroupName,
            Programs = g.BindSimilaEducationalProgramInGroups.Select(b => new EpSimpleDto
            {
                Id = b.EducationalProgram.IdEducationalProgram,
                Name = b.EducationalProgram.NameEducationalProgram
            }).ToList()
        }).ToList();
    }

    public async Task RemoveEpFromGroupAsync(Guid groupId, Guid epId)
    {
        await _repository.RemoveFromGroupAsync(groupId, epId);
        await _repository.SaveChangesAsync();
    }

    private List<string> ExtractKeywords(EducationalProgram p)
    {
        var text = $"{p.NameEducationalProgram} {p.Goals} {p.Subject} {p.TheoreticalContent} {p.Methodics}";
        return Tokenize(text);
    }

    private List<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new List<string>();
        text = Regex.Replace(text, "<.*?>", string.Empty);
        var words = Regex.Matches(text.ToLower(), @"\w{4,}")
            .Select(m => m.Value)
            .Where(w => !StopWords.Contains(w))
            .Distinct()
            .Take(15)
            .ToList();
        return words;
    }

    private bool IsSimilar(EducationalProgram a, EducationalProgram b)
    {
        if (a.IdEducationalProgram == b.IdEducationalProgram) return true;

        var nameWordsA = a.NameEducationalProgram.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 3).ToHashSet();
        var nameWordsB = b.NameEducationalProgram.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 3).ToHashSet();
        int nameOverlap = nameWordsA.Intersect(nameWordsB).Count();

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
    };
}
