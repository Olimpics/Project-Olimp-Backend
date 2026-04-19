using Microsoft.EntityFrameworkCore;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Infrastructure.Redis;
using OlimpBack.Utils;

namespace OlimpBack.Application.Permissions;

public class RoleMaskService : IRoleMaskService
{
    private static readonly TimeSpan RoleMaskTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan UserMaskTtl = TimeSpan.FromMinutes(5);

    private readonly AppDbContext _context;
    private readonly IRbacCacheService _cache;
    private readonly ILogger<RoleMaskService> _logger;

    public RoleMaskService(AppDbContext context, IRbacCacheService cache, ILogger<RoleMaskService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<long> RecalculateRoleMaskAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var bitIndexes = await _context.BindRolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => x.Permission.BitIndex)
            .ToListAsync(cancellationToken);

        var newMask = PermissionMaskHelper.BuildMask(bitIndexes);

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.IdRole == roleId, cancellationToken);
        if (role == null)
            return 0;

        role.PermissionsMask = newMask;
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.SetLongAsync(GetRoleMaskKey(roleId), newMask, RoleMaskTtl, cancellationToken);

        await InvalidateUserMaskCacheByRoleAsync(roleId, cancellationToken);

        return newMask;
    }

    public async Task<long> GetRoleMaskAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetRoleMaskKey(roleId);
        var cached = await _cache.GetLongAsync(cacheKey, cancellationToken);
        if (cached.HasValue)
            return cached.Value;

        var roleMask = await _context.Roles
            .AsNoTracking()
            .Where(r => r.IdRole == roleId)
            .Select(r => r.PermissionsMask)
            .FirstOrDefaultAsync(cancellationToken);

        await _cache.SetLongAsync(cacheKey, roleMask, RoleMaskTtl, cancellationToken);
        return roleMask;
    }

    public async Task<long> GetUserPermissionsMaskAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetUserMaskKey(userId);
        var cached = await _cache.GetLongAsync(cacheKey, cancellationToken);
        if (cached.HasValue)
            return cached.Value;

        long mask;
        try
        {
            // Primary path: supports many roles via user_roles and role inheritance via parent_role_id.
            mask = await _context.Database.SqlQuery<long>($$"""
                WITH RECURSIVE role_tree AS (
                    SELECT r.idRole, r.parent_role_id, r.permissions_mask
                    FROM Role r
                    WHERE r.idRole IN (
                        SELECT ur.role_id FROM user_roles ur WHERE ur.user_id = {{userId}}
                        UNION
                        SELECT u.RoleId FROM Users u WHERE u.IdUsers = {{userId}}
                    )
                    UNION ALL
                    SELECT r2.idRole, r2.parent_role_id, r2.permissions_mask
                    FROM Role r2
                    JOIN role_tree rt ON r2.idRole = rt.parent_role_id
                )
                SELECT COALESCE(BIT_OR(COALESCE(permissions_mask, 0)), 0) AS Value
                FROM role_tree;
                """)
                .SingleAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Falling back to single-role permission mask query for user {UserId}. Check user_roles/parent_role_id schema.",
                userId);

            mask = await _context.Database.SqlQuery<long>($$"""
                SELECT COALESCE(r.permissions_mask, 0) AS Value
                FROM Users u
                JOIN Role r ON r.idRole = u.RoleId
                WHERE u.IdUsers = {{userId}}
                LIMIT 1;
                """)
                .SingleAsync(cancellationToken);
        }

        await _cache.SetLongAsync(cacheKey, mask, UserMaskTtl, cancellationToken);
        return mask;
    }

    private async Task InvalidateUserMaskCacheByRoleAsync(int roleId, CancellationToken cancellationToken)
    {
        var userIds = await _context.Users
            .AsNoTracking()
            .Where(u => u.RoleId == roleId)
            .Select(u => u.IdUsers)
            .ToListAsync(cancellationToken);

        foreach (var userId in userIds)
            await _cache.RemoveAsync(GetUserMaskKey(userId), cancellationToken);
    }

    private static string GetRoleMaskKey(int roleId) => $"rbac:role_mask:{roleId}";
    private static string GetUserMaskKey(int userId) => $"rbac:user_mask:{userId}";
}
