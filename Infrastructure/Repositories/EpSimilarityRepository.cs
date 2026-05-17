using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Infrastructure.Repositories;

public interface IEpSimilarityRepository
{
    Task<List<EducationalProgram>> GetAllProgramsAsync();
    Task<List<GroupSimilarEducationalProgram>> GetAllGroupsWithProgramsAsync();
    Task<List<GroupSimilarEducationalProgram>> GetGroupsByEpIdAsync(Guid epId);
    Task ClearAllGroupsAsync();
    Task AddGroupAsync(GroupSimilarEducationalProgram group);
    Task AddBindingAsync(BindSimilaEducationalProgramInGroup binding);
    Task RemoveFromGroupAsync(Guid groupId, Guid epId);
    Task SaveChangesAsync();
}

public class EpSimilarityRepository : IEpSimilarityRepository
{
    private readonly AppDbContext _context;

    public EpSimilarityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationalProgram>> GetAllProgramsAsync()
    {
        return await _context.EducationalPrograms.ToListAsync();
    }

    public async Task<List<GroupSimilarEducationalProgram>> GetAllGroupsWithProgramsAsync()
    {
        return await _context.GroupSimilarEducationalPrograms
            .Include(g => g.BindSimilaEducationalProgramInGroups)
            .ThenInclude(b => b.EducationalProgram)
            .ToListAsync();
    }

    public async Task<List<GroupSimilarEducationalProgram>> GetGroupsByEpIdAsync(Guid epId)
    {
        var groupIds = await _context.BindSimilaEducationalProgramInGroups
            .Where(b => b.EducationalProgramId == epId)
            .Select(b => b.GroupId)
            .ToListAsync();

        return await _context.GroupSimilarEducationalPrograms
            .Where(g => groupIds.Contains(g.IdGroup))
            .Include(g => g.BindSimilaEducationalProgramInGroups)
            .ThenInclude(b => b.EducationalProgram)
            .ToListAsync();
    }

    public async Task ClearAllGroupsAsync()
    {
        _context.BindSimilaEducationalProgramInGroups.RemoveRange(_context.BindSimilaEducationalProgramInGroups);
        _context.GroupSimilarEducationalPrograms.RemoveRange(_context.GroupSimilarEducationalPrograms);
        await _context.SaveChangesAsync();
    }

    public async Task AddGroupAsync(GroupSimilarEducationalProgram group)
    {
        _context.GroupSimilarEducationalPrograms.Add(group);
    }

    public async Task AddBindingAsync(BindSimilaEducationalProgramInGroup binding)
    {
        _context.BindSimilaEducationalProgramInGroups.Add(binding);
    }

    public async Task RemoveFromGroupAsync(Guid groupId, Guid epId)
    {
        var binding = await _context.BindSimilaEducationalProgramInGroups
            .FirstOrDefaultAsync(b => b.GroupId == groupId && b.EducationalProgramId == epId);
        if (binding != null)
        {
            _context.BindSimilaEducationalProgramInGroups.Remove(binding);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
