using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Infrastructure.Repositories;

public interface ISimilarityRepository
{
    Task<List<SelectiveDiscipline>> GetAllDisciplinesWithDetailsAsync();
    Task<List<GroupSimilarSelective>> GetAllGroupsWithDisciplinesAsync();
    Task<List<GroupSimilarSelective>> GetGroupsBySelectiveIdAsync(Guid selectiveId);
    Task ClearAllGroupsAsync();
    Task AddGroupAsync(GroupSimilarSelective group);
    Task AddBindingAsync(BindSimilarSelectiveInGroup binding);
    Task<List<Guid>> GetDisciplinesInGroupAsync(Guid groupId);
    Task RemoveFromGroupAsync(Guid groupId, Guid selectiveId);
    Task SaveChangesAsync();
}

public class SimilarityRepository : ISimilarityRepository
{
    private readonly AppDbContext _context;

    public SimilarityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SelectiveDiscipline>> GetAllDisciplinesWithDetailsAsync()
    {
        return await _context.SelectiveDisciplines
            .Include(sd => sd.SelectiveDetail)
            .ToListAsync();
    }

    public async Task<List<GroupSimilarSelective>> GetAllGroupsWithDisciplinesAsync()
    {
        return await _context.GroupSimilarSelectives
            .Include(g => g.BindSimilarSelectiveInGroups)
            .ThenInclude(b => b.Selective)
            .ToListAsync();
    }

    public async Task<List<GroupSimilarSelective>> GetGroupsBySelectiveIdAsync(Guid selectiveId)
    {
        var groupIds = await _context.BindSimilarSelectiveInGroups
            .Where(b => b.SelectiveId == selectiveId)
            .Select(b => b.GroupId)
            .ToListAsync();

        return await _context.GroupSimilarSelectives
            .Where(g => groupIds.Contains(g.IdGroup))
            .Include(g => g.BindSimilarSelectiveInGroups)
            .ThenInclude(b => b.Selective)
            .ToListAsync();
    }

    public async Task ClearAllGroupsAsync()
    {
        _context.BindSimilarSelectiveInGroups.RemoveRange(_context.BindSimilarSelectiveInGroups);
        _context.GroupSimilarSelectives.RemoveRange(_context.GroupSimilarSelectives);
        await _context.SaveChangesAsync();
    }

    public async Task AddGroupAsync(GroupSimilarSelective group)
    {
        _context.GroupSimilarSelectives.Add(group);
    }

    public async Task AddBindingAsync(BindSimilarSelectiveInGroup binding)
    {
        _context.BindSimilarSelectiveInGroups.Add(binding);
    }

    public async Task<List<Guid>> GetDisciplinesInGroupAsync(Guid groupId)
    {
        return await _context.BindSimilarSelectiveInGroups
            .Where(b => b.GroupId == groupId)
            .Select(b => b.SelectiveId)
            .ToListAsync();
    }

    public async Task RemoveFromGroupAsync(Guid groupId, Guid selectiveId)
    {
        var binding = await _context.BindSimilarSelectiveInGroups
            .FirstOrDefaultAsync(b => b.GroupId == groupId && b.SelectiveId == selectiveId);
        if (binding != null)
        {
            _context.BindSimilarSelectiveInGroups.Remove(binding);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
