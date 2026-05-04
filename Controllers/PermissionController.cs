using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Data;
using OlimpBack.Models;
using OlimpBack.Utils;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IRoleMaskService _roleMaskService;

    public PermissionController(AppDbContext context, IRoleMaskService roleMaskService)
    {
        _context = context;
        _roleMaskService = roleMaskService;
    }

    [HttpGet]
    [RequirePermission(RbacPermissions.PermissionsRead)]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
    {
        var permissions = await _context.Permissions
            .FromSqlRaw(@"
                SELECT
                    p.""idPermission"" AS id,
                    p.code,
                    p.""bitIndex"" AS bit_index
                FROM ""Permissions"" p")
            .AsNoTracking()
            .OrderBy(p => p.BitIndex)
            .Select(p => ToDto(p))
            .ToListAsync();

        return Ok(permissions);
    }

    [HttpGet("{id:int}")]
    [RequirePermission(RbacPermissions.PermissionsRead)]
    public async Task<ActionResult<PermissionDto>> GetPermission(int id)
    {
        var permission = await GetPermissionEntityAsync(id);
        if (permission == null)
            return NotFound();

        return Ok(ToDto(permission));
    }

    [HttpPost]
    [RequirePermission(RbacPermissions.PermissionsCreate)]
    public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionDto permissionDto)
    {
        if (permissionDto.BitIndex < PermissionMaskHelper.MinBitIndex || permissionDto.BitIndex > PermissionMaskHelper.MaxBitIndex)
            return BadRequest($"BitIndex must be in range [{PermissionMaskHelper.MinBitIndex}, {PermissionMaskHelper.MaxBitIndex}].");

        var code = BuildCode(permissionDto.TypePermission, permissionDto.TableName);
        if (await PermissionCodeExistsAsync(code))
            return BadRequest("Permission with this code already exists.");

        if (await PermissionBitIndexExistsAsync(permissionDto.BitIndex))
            return BadRequest("BitIndex is already used by another permission.");

        await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""Permissions"" (code, ""bitIndex"")
            VALUES ({code}, {permissionDto.BitIndex})");

        var permission = await GetPermissionByCodeAsync(code);

        return CreatedAtAction(nameof(GetPermission), new { id = permission!.IdPermission }, ToDto(permission));
    }

    [HttpPut("{id:int}")]
    [RequirePermission(RbacPermissions.PermissionsUpdate)]
    public async Task<IActionResult> UpdatePermission(int id, UpdatePermissionDto permissionDto)
    {
        if (id != permissionDto.IdPermissions)
            return BadRequest();

        if (permissionDto.BitIndex < PermissionMaskHelper.MinBitIndex || permissionDto.BitIndex > PermissionMaskHelper.MaxBitIndex)
            return BadRequest($"BitIndex must be in range [{PermissionMaskHelper.MinBitIndex}, {PermissionMaskHelper.MaxBitIndex}].");

        var permission = await GetPermissionEntityAsync(id);
        if (permission == null)
            return NotFound();

        var newCode = BuildCode(permissionDto.TypePermission, permissionDto.TableName);
        if (await PermissionCodeExistsAsync(newCode, id))
            return BadRequest("Permission with this code already exists.");

        if (await PermissionBitIndexExistsAsync(permissionDto.BitIndex, id))
            return BadRequest("BitIndex is already used by another permission.");

        var affectedRoleIds = await _context.Roles
            .FromSqlInterpolated($@"
                SELECT
                    r.""idRole"" AS id_role,
                    r.name,
                    r.""parentRoleId"" AS parent_role_id,
                    r.""permissionsMask"" AS permissions_mask
                FROM ""Roles"" r
                INNER JOIN ""RolePermissions"" rp ON rp.""RoleId"" = r.""idRole""
                WHERE rp.""PermissionId"" = {id}")
            .Select(role => role.IdRole)
            .Distinct()
            .ToListAsync();

        await _context.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE ""Permissions""
            SET code = {newCode}, ""bitIndex"" = {permissionDto.BitIndex}
            WHERE ""idPermission"" = {id}");

        foreach (var roleId in affectedRoleIds)
            await _roleMaskService.RecalculateRoleMaskAsync(roleId);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission(RbacPermissions.PermissionsDelete)]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var permission = await GetPermissionEntityAsync(id);
        if (permission == null)
            return NotFound();

        var affectedRoleIds = await _context.Roles
            .FromSqlInterpolated($@"
                SELECT
                    r.""idRole"" AS id_role,
                    r.name,
                    r.""parentRoleId"" AS parent_role_id,
                    r.""permissionsMask"" AS permissions_mask
                FROM ""Roles"" r
                INNER JOIN ""RolePermissions"" rp ON rp.""RoleId"" = r.""idRole""
                WHERE rp.""PermissionId"" = {id}")
            .Select(role => role.IdRole)
            .Distinct()
            .ToListAsync();

        await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM ""Permissions""
            WHERE ""idPermission"" = {id}");

        foreach (var roleId in affectedRoleIds)
            await _roleMaskService.RecalculateRoleMaskAsync(roleId);

        return NoContent();
    }

    private static PermissionDto ToDto(Permission permission)
    {
        var parts = permission.Code.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
        return new PermissionDto
        {
            IdPermissions = permission.IdPermission,
            TableName = parts.Length > 0 ? parts[0] : permission.Code,
            TypePermission = parts.Length > 1 ? parts[1] : string.Empty,
            BitIndex = permission.BitIndex
        };
    }

    private static string BuildCode(string typePermission, string tableName)
    {
        if (tableName.Contains('.'))
            return tableName;

        return $"{tableName}.{typePermission}";
    }

    private async Task<Permission?> GetPermissionEntityAsync(int id)
    {
        return await _context.Permissions
            .FromSqlInterpolated($@"
                SELECT
                    p.""idPermission"" AS id,
                    p.code,
                    p.""bitIndex"" AS bit_index
                FROM ""Permissions"" p
                WHERE p.""idPermission"" = {id}")
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    private async Task<Permission?> GetPermissionByCodeAsync(string code)
    {
        return await _context.Permissions
            .FromSqlInterpolated($@"
                SELECT
                    p.""idPermission"" AS id,
                    p.code,
                    p.""bitIndex"" AS bit_index
                FROM ""Permissions"" p
                WHERE p.code = {code}")
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    private async Task<bool> PermissionCodeExistsAsync(string code, int? exceptId = null)
    {
        var permission = await GetPermissionByCodeAsync(code);
        return permission != null && permission.IdPermission != exceptId;
    }

    private async Task<bool> PermissionBitIndexExistsAsync(int bitIndex, int? exceptId = null)
    {
        var permission = await _context.Permissions
            .FromSqlInterpolated($@"
                SELECT
                    p.""idPermission"" AS id,
                    p.code,
                    p.""bitIndex"" AS bit_index
                FROM ""Permissions"" p
                WHERE p.""bitIndex"" = {bitIndex}")
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return permission != null && permission.IdPermission != exceptId;
    }
}
