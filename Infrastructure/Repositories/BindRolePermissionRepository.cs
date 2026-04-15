using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IBindRolePermissionRepository
{
    Task<IEnumerable<BindRolePermissionDto>> GetAllDtoAsync();
    Task<BindRolePermissionDto?> GetDtoByIdAsync(int id);
    Task<BindRolePermission?> GetEntityByIdAsync(int id);
    Task<bool> ExistsRoleAsync(int roleId);
    Task<bool> ExistsPermissionAsync(int permissionId);
    Task<bool> BindingExistsAsync(int roleId, int permissionId, int? excludeId = null);
    Task AddAsync(BindRolePermission binding);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class BindRolePermissionRepository : IBindRolePermissionRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BindRolePermissionRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BindRolePermissionDto>> GetAllDtoAsync()
    {
        return await _context.BindRolePermissions
            .AsNoTracking()
            .ProjectTo<BindRolePermissionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<BindRolePermissionDto?> GetDtoByIdAsync(int id)
    {
        return await _context.BindRolePermissions
            .AsNoTracking()
            .Where(b => b.IdBindRolePermission == id)
            .ProjectTo<BindRolePermissionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<BindRolePermission?> GetEntityByIdAsync(int id)
    {
        return await _context.BindRolePermissions.FindAsync(id);
    }

    public async Task<bool> ExistsRoleAsync(int roleId)
    {
        return await _context.Roles.AnyAsync(r => r.IdRole == roleId);
    }

    public async Task<bool> ExistsPermissionAsync(int permissionId)
    {
        return await _context.Permissions.AnyAsync(p => p.IdPermissions == permissionId);
    }

    public async Task<bool> BindingExistsAsync(int roleId, int permissionId, int? excludeId = null)
    {
        var query = _context.BindRolePermissions.Where(b => b.RoleId == roleId && b.PermissionId == permissionId);

        if (excludeId.HasValue)
        {
            query = query.Where(b => b.IdBindRolePermission != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(BindRolePermission binding)
    {
        await _context.BindRolePermissions.AddAsync(binding);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _context.BindRolePermissions
            .Where(b => b.IdBindRolePermission == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}