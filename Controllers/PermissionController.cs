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
        var permission = await _context.Permissions.FindAsync(id);
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
        if (await _context.Permissions.AnyAsync(x => x.Code == code))
            return BadRequest("Permission with this code already exists.");

        if (await _context.Permissions.AnyAsync(x => x.BitIndex == permissionDto.BitIndex))
            return BadRequest("BitIndex is already used by another permission.");

        var permission = new Permission
        {
            Code = code,
            BitIndex = permissionDto.BitIndex
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPermission), new { id = permission.IdPermission }, ToDto(permission));
    }

    [HttpPut("{id:int}")]
    [RequirePermission(RbacPermissions.PermissionsUpdate)]
    public async Task<IActionResult> UpdatePermission(int id, UpdatePermissionDto permissionDto)
    {
        if (id != permissionDto.IdPermissions)
            return BadRequest();

        if (permissionDto.BitIndex < PermissionMaskHelper.MinBitIndex || permissionDto.BitIndex > PermissionMaskHelper.MaxBitIndex)
            return BadRequest($"BitIndex must be in range [{PermissionMaskHelper.MinBitIndex}, {PermissionMaskHelper.MaxBitIndex}].");

        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null)
            return NotFound();

        var newCode = BuildCode(permissionDto.TypePermission, permissionDto.TableName);
        if (await _context.Permissions.AnyAsync(x => x.IdPermission != id && x.Code == newCode))
            return BadRequest("Permission with this code already exists.");

        if (await _context.Permissions.AnyAsync(x => x.IdPermission != id && x.BitIndex == permissionDto.BitIndex))
            return BadRequest("BitIndex is already used by another permission.");

        var affectedRoleIds = await _context.RolePermissions
            .Where(x => x.PermissionId == id)
            .Select(x => x.RoleId)
            .Distinct()
            .ToListAsync();

        permission.Code = newCode;
        permission.BitIndex = permissionDto.BitIndex;
        await _context.SaveChangesAsync();

        foreach (var roleId in affectedRoleIds)
            await _roleMaskService.RecalculateRoleMaskAsync(roleId);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission(RbacPermissions.PermissionsDelete)]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null)
            return NotFound();

        var affectedRoleIds = await _context.RolePermissions
            .Where(x => x.PermissionId == id)
            .Select(x => x.RoleId)
            .Distinct()
            .ToListAsync();

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();

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
}
