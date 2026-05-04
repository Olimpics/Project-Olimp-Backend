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
        var result = new List<BindRolePermissionDto>();
        var roles = await _context.Roles
            .FromSqlRaw(@"
                SELECT
                    r.""idRole"" AS id_role,
                    r.name,
                    r.""parentRoleId"" AS parent_role_id,
                    r.""permissionsMask"" AS permissions_mask
                FROM ""Roles"" r")
            .AsNoTracking()
            .OrderBy(role => role.IdRole)
            .ToListAsync();

        foreach (var role in roles)
        {
            var permissions = await GetPermissionsByRoleAsync(role.IdRole);
            result.AddRange(permissions.Select(permission => ToDto(role, permission)));
        }

        return result
            .OrderBy(dto => dto.RoleId)
            .ThenBy(dto => dto.PermissionId)
            .ToList();
    }

    public async Task<BindRolePermissionDto?> GetDtoAsync(int roleId, int permissionId)
    {
        var role = await _context.Roles
            .FromSqlInterpolated($@"
                SELECT
                    r.""idRole"" AS id_role,
                    r.name,
                    r.""parentRoleId"" AS parent_role_id,
                    r.""permissionsMask"" AS permissions_mask
                FROM ""Roles"" r
                WHERE r.""idRole"" = {roleId}")
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (role == null)
            return null;

        var permission = await GetPermissionByRoleAsync(roleId, permissionId);
        return permission == null ? null : ToDto(role, permission);
    }

    public async Task<RolePermission?> GetEntityAsync(int roleId, int permissionId)
    {
        return await BindingExistsAsync(roleId, permissionId)
            ? new RolePermission { RoleId = roleId, PermissionId = permissionId }
            : null;
    }

    public async Task<bool> ExistsRoleAsync(int roleId)
    {
        return await _context.Roles
            .FromSqlInterpolated($@"
                SELECT
                    r.""idRole"" AS id_role,
                    r.name,
                    r.""parentRoleId"" AS parent_role_id,
                    r.""permissionsMask"" AS permissions_mask
                FROM ""Roles"" r
                WHERE r.""idRole"" = {roleId}")
            .AsNoTracking()
            .AnyAsync();
    }

    public async Task<bool> ExistsPermissionAsync(int permissionId)
    {
        return await _context.Permissions
            .FromSqlInterpolated($@"
                SELECT
                    p.""idPermission"" AS id,
                    p.code,
                    p.""bitIndex"" AS bit_index
                FROM ""Permissions"" p
                WHERE p.""idPermission"" = {permissionId}")
            .AsNoTracking()
            .AnyAsync();
    }

    public async Task<bool> BindingExistsAsync(int roleId, int permissionId)
    {
        return await GetPermissionByRoleAsync(roleId, permissionId) != null;
    }

    public async Task AddAsync(RolePermission binding)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""RolePermissions"" (""RoleId"", ""PermissionId"")
            VALUES ({binding.RoleId}, {binding.PermissionId})");
    }

    public async Task<int> DeleteAsync(int roleId, int permissionId)
    {
        return await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM ""RolePermissions""
            WHERE ""RoleId"" = {roleId} AND ""PermissionId"" = {permissionId}");
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private async Task<List<Permission>> GetPermissionsByRoleAsync(int roleId)
    {
        return await _context.Permissions
            .FromSqlInterpolated($@"
                SELECT
                    p.""idPermission"" AS id,
                    p.code,
                    p.""bitIndex"" AS bit_index
                FROM ""Permissions"" p
                INNER JOIN ""RolePermissions"" rp ON rp.""PermissionId"" = p.""idPermission""
                WHERE rp.""RoleId"" = {roleId}")
            .AsNoTracking()
            .OrderBy(permission => permission.BitIndex)
            .ToListAsync();
    }

    private async Task<Permission?> GetPermissionByRoleAsync(int roleId, int permissionId)
    {
        return await _context.Permissions
            .FromSqlInterpolated($@"
                SELECT
                    p.""idPermission"" AS id,
                    p.code,
                    p.""bitIndex"" AS bit_index
                FROM ""Permissions"" p
                INNER JOIN ""RolePermissions"" rp ON rp.""PermissionId"" = p.""idPermission""
                WHERE rp.""RoleId"" = {roleId} AND rp.""PermissionId"" = {permissionId}")
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    private static BindRolePermissionDto ToDto(Role role, Permission permission)
    {
        return new BindRolePermissionDto
        {
            IdBindRolePermission = permission.IdPermission,
            RoleId = role.IdRole,
            RoleName = role.Name,
            PermissionId = permission.IdPermission,
            TypePermission = GetPermissionAction(permission.Code),
            TableName = GetPermissionResource(permission.Code)
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
