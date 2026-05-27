using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Utils;

namespace OlimpBack.Application.Permissions;

public class PermissionDelegationService : IPermissionDelegationService
{
    private readonly AppDbContext _context;
    private readonly IRoleMaskService _roleMaskService;
    private readonly IRoleKindService _roleKind;

    public PermissionDelegationService(
        AppDbContext context,
        IRoleMaskService roleMaskService,
        IRoleKindService roleKind)
    {
        _context = context;
        _roleMaskService = roleMaskService;
        _roleKind = roleKind;
    }

    public async Task<int> GetUserHierarchyLevelAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roles = await _roleKind.GetUserRoleSnapshotsAsync(userId, cancellationToken);
        return RoleHierarchyCatalog.GetEffectiveGranterLevel(roles);
    }

    public async Task<int> GetRoleHierarchyLevelAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleName = await _context.Roles
            .AsNoTracking()
            .Where(r => r.IdRole == roleId)
            .Select(r => r.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return roleName == null
            ? int.MinValue
            : RoleHierarchyCatalog.ResolveLevel(roleName);
    }

    public async Task<bool> CanGrantAsync(
        Guid granterUserId,
        Guid targetRoleId,
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        var granterRoles = await _roleKind.GetUserRoleSnapshotsAsync(granterUserId, cancellationToken);
        var targetRole = await _roleKind.GetRoleSnapshotAsync(targetRoleId, cancellationToken);

        if (targetRole == null)
            return false;

        if (!RoleHierarchyCatalog.CanManageRolePermissions(granterRoles, targetRole.Value))
            return false;

        var permission = await _context.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPermission == permissionId, cancellationToken);

        if (permission == null)
            return false;

        if (!RbacPermissionCatalog.All.Any(definition => definition.BitIndex == permission.BitIndex))
            return false;

        var granterMask = await _roleMaskService.GetUserPermissionsMaskAsync(granterUserId, cancellationToken);
        var requiredMask = PermissionMaskHelper.ToMask(permission.BitIndex);

        return PermissionMaskHelper.HasAll(granterMask, requiredMask);
    }

    public async Task<IReadOnlyList<GrantablePermissionDto>> GetGrantablePermissionsAsync(
        Guid granterUserId,
        Guid targetRoleId,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.Roles.AsNoTracking().AnyAsync(r => r.IdRole == targetRoleId, cancellationToken))
            return Array.Empty<GrantablePermissionDto>();

        var permissions = await _context.Permissions
            .AsNoTracking()
            .OrderBy(p => p.BitIndex)
            .ToListAsync(cancellationToken);

        var result = new List<GrantablePermissionDto>();

        foreach (var permission in permissions)
        {
            if (!await CanGrantAsync(granterUserId, targetRoleId, permission.IdPermission, cancellationToken))
                continue;

            if (!RbacPermissionCatalog.All.Any(definition => definition.BitIndex == permission.BitIndex))
                continue;

            result.Add(new GrantablePermissionDto
            {
                PermissionId = permission.IdPermission,
                Code = permission.Code,
                BitIndex = permission.BitIndex,
                MinGrantLevel = 0,
                TypePermission = GetPermissionAction(permission.Code),
                TableName = GetPermissionResource(permission.Code)
            });
        }

        return result;
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
