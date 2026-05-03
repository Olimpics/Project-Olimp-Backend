using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IBindRolePermissionRepository
{
    Task<IEnumerable<BindRolePermissionDto>> GetAllDtoAsync();
    Task<BindRolePermissionDto?> GetDtoAsync(int roleId, int permissionId);
    Task<RolePermission?> GetEntityAsync(int roleId, int permissionId);
    Task<bool> ExistsRoleAsync(int roleId);
    Task<bool> ExistsPermissionAsync(int permissionId);
    Task<bool> BindingExistsAsync(int roleId, int permissionId);
    Task AddAsync(RolePermission binding);
    Task<int> DeleteAsync(int roleId, int permissionId);
    Task SaveChangesAsync();
}

public class BindRolePermissionRepository : IBindRolePermissionRepository
{
    private readonly AppDbContext _context;

    public BindRolePermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BindRolePermissionDto>> GetAllDtoAsync()
    {
        return await _context.RolePermissions
            .AsNoTracking()
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .OrderBy(rp => rp.RoleId)
            .ThenBy(rp => rp.Permission.BitIndex)
            .Select(rp => ToDto(rp))
            .ToListAsync();
    }

    public async Task<BindRolePermissionDto?> GetDtoAsync(int roleId, int permissionId)
    {
        return await _context.RolePermissions
            .AsNoTracking()
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)
            .Select(rp => ToDto(rp))
            .FirstOrDefaultAsync();
    }

    public async Task<RolePermission?> GetEntityAsync(int roleId, int permissionId)
    {
        return await _context.RolePermissions.FindAsync(roleId, permissionId);
    }

    public async Task<bool> ExistsRoleAsync(int roleId)
    {
        return await _context.Roles.AnyAsync(r => r.IdRole == roleId);
    }

    public async Task<bool> ExistsPermissionAsync(int permissionId)
    {
        return await _context.Permissions.AnyAsync(p => p.IdPermission == permissionId);
    }

    public async Task<bool> BindingExistsAsync(int roleId, int permissionId)
    {
        return await _context.RolePermissions.AnyAsync(b => b.RoleId == roleId && b.PermissionId == permissionId);
    }

    public async Task AddAsync(RolePermission binding)
    {
        await _context.RolePermissions.AddAsync(binding);
    }

    public async Task<int> DeleteAsync(int roleId, int permissionId)
    {
        return await _context.RolePermissions
            .Where(b => b.RoleId == roleId && b.PermissionId == permissionId)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private static BindRolePermissionDto ToDto(RolePermission binding)
    {
        return new BindRolePermissionDto
        {
            IdBindRolePermission = binding.PermissionId,
            RoleId = binding.RoleId,
            RoleName = binding.Role.Name,
            PermissionId = binding.PermissionId,
            TypePermission = GetPermissionAction(binding.Permission.Code),
            TableName = GetPermissionResource(binding.Permission.Code)
        };
    }

    private static string GetPermissionResource(string code)
    {
        var parts = code.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : code;
    }

    private static string GetPermissionAction(string code)
    {
        var parts = code.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : string.Empty;
    }
}
