using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Utils;

namespace OlimpBack.Application.Permissions;

public class RoleMaskService : IRoleMaskService
{
    private readonly AppDbContext _context;

    public RoleMaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<long> RecalculateRoleMaskAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.IdRole == roleId, cancellationToken);

        if (role == null)
            return 0;

        var newMask = PermissionMaskHelper.BuildMask(
            role.RolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission.BitIndex));

        role.PermissionsMask = newMask;
        await _context.SaveChangesAsync(cancellationToken);

        // TEMPORARY: Redis cache is disabled for local development until Redis is available.
        // await _cache.SetLongAsync(GetRoleMaskKey(roleId), newMask, RoleMaskTtl, cancellationToken);
        // await InvalidateUserMaskCacheByRoleAsync(roleId, cancellationToken);

        return newMask;
    }

    public async Task<long> GetRoleMaskAsync(int roleId, CancellationToken cancellationToken = default)
    {
        // TEMPORARY: Redis cache is disabled for local development until Redis is available.
        // var cacheKey = GetRoleMaskKey(roleId);
        // var cached = await _cache.GetLongAsync(cacheKey, cancellationToken);
        // if (cached.HasValue)
        //     return cached.Value;

        var roleMask = await _context.Roles
            .AsNoTracking()
            .Where(r => r.IdRole == roleId)
            .Select(r => r.PermissionsMask ?? 0L)
            .FirstOrDefaultAsync(cancellationToken);

        // TEMPORARY: Redis cache is disabled for local development until Redis is available.
        // await _cache.SetLongAsync(cacheKey, roleMask, RoleMaskTtl, cancellationToken);
        return roleMask;
    }

    public async Task<long> GetUserPermissionsMaskAsync(int userId, CancellationToken cancellationToken = default)
    {
        // TEMPORARY: Redis cache is disabled for local development until Redis is available.
        // var cacheKey = GetUserMaskKey(userId);
        // var cached = await _cache.GetLongAsync(cacheKey, cancellationToken);
        // if (cached.HasValue)
        //     return cached.Value;

        var roleIds = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        long combined = 0;
        foreach (var roleId in roleIds)
            combined |= await GetRoleMaskWithAncestorsAsync(roleId, cancellationToken);

        // TEMPORARY: Redis cache is disabled for local development until Redis is available.
        // await _cache.SetLongAsync(cacheKey, combined, UserMaskTtl, cancellationToken);
        return combined;
    }

    private async Task<long> GetRoleMaskWithAncestorsAsync(int roleId, CancellationToken cancellationToken)
    {
        long mask = 0;
        var currentRoleId = roleId;
        var visited = new HashSet<int>();

        while (visited.Add(currentRoleId))
        {
            var roleInfo = await _context.Roles
                .AsNoTracking()
                .Where(r => r.IdRole == currentRoleId)
                .Select(r => new
                {
                    Mask = r.PermissionsMask ?? 0L,
                    r.ParentRoleId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (roleInfo == null)
                break;

            mask |= roleInfo.Mask;

            if (!roleInfo.ParentRoleId.HasValue)
                break;

            currentRoleId = roleInfo.ParentRoleId.Value;
        }

        return mask;
    }

    // TEMPORARY: Redis cache helpers are disabled for local development until Redis is available.
    // private async Task InvalidateUserMaskCacheByRoleAsync(int roleId, CancellationToken cancellationToken) { ... }
    // private static string GetRoleMaskKey(int roleId) => $"rbac:role_mask:{roleId}";
    // private static string GetUserMaskKey(int userId) => $"rbac:user_mask:{userId}";
}
//using Microsoft.EntityFrameworkCore;
//using OlimpBack.Infrastructure.Database;
//using OlimpBack.Infrastructure.Redis;
//using OlimpBack.Models;
//using OlimpBack.Utils;
//using OlimpBack.Data;

//namespace OlimpBack.Application.Permissions;

//public class RoleMaskService : IRoleMaskService
//{
//    private static readonly TimeSpan RoleMaskTtl = TimeSpan.FromMinutes(30);
//    private static readonly TimeSpan UserMaskTtl = TimeSpan.FromMinutes(5);

//    private readonly AppDbContext _context;
//    private readonly IRbacCacheService _cache;
//    public RoleMaskService(AppDbContext context, IRbacCacheService cache)
//    {
//        _context = context;
//        _cache = cache;
//    }

//    public async Task<long> RecalculateRoleMaskAsync(int roleId, CancellationToken cancellationToken = default)
//    {
//        var role = await _context.Set<Role1>()
//            .Include(r => r.Permissions)
//            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

//        if (role == null)
//            return 0;

//        var newMask = PermissionMaskHelper.BuildMask(role.Permissions.Select(p => p.BitIndex));
//        role.PermissionsMask = newMask;

//        await _context.SaveChangesAsync(cancellationToken);

//        await _cache.SetLongAsync(GetRoleMaskKey(roleId), newMask, RoleMaskTtl, cancellationToken);
//        await InvalidateUserMaskCacheByRoleAsync(roleId, cancellationToken);

//        return newMask;
//    }

//    public async Task<long> GetRoleMaskAsync(int roleId, CancellationToken cancellationToken = default)
//    {
//        var cacheKey = GetRoleMaskKey(roleId);
//        var cached = await _cache.GetLongAsync(cacheKey, cancellationToken);
//        if (cached.HasValue)
//            return cached.Value;

//        var roleMask = await _context.Set<Role1>()
//            .AsNoTracking()
//            .Where(r => r.Id == roleId)
//            .Select(r => r.PermissionsMask ?? 0L)
//            .FirstOrDefaultAsync(cancellationToken);

//        await _cache.SetLongAsync(cacheKey, roleMask, RoleMaskTtl, cancellationToken);
//        return roleMask;
//    }

//    public async Task<long> GetUserPermissionsMaskAsync(int userId, CancellationToken cancellationToken = default)
//    {
//        var cacheKey = GetUserMaskKey(userId);
//        var cached = await _cache.GetLongAsync(cacheKey, cancellationToken);
//        if (cached.HasValue)
//            return cached.Value;

//        var user = await _context.Users
//            .AsNoTracking()
//            .Include(u => u.Roles)
//            .FirstOrDefaultAsync(u => u.IdUser == userId, cancellationToken);
//        if (user == null)
//            return 0;

//        var roleIds = new HashSet<int> { user.Roleid };
//        foreach (var r in user.Roles)
//            roleIds.Add(r.Id);

//        long combined = 0;
//        foreach (var rid in roleIds)
//            combined |= await GetRoleMaskWithAncestorsAsync(rid, cancellationToken);

//        await _cache.SetLongAsync(cacheKey, combined, UserMaskTtl, cancellationToken);
//        return combined;
//    }

//    private async Task<long> GetRoleMaskWithAncestorsAsync(int roleId, CancellationToken cancellationToken)
//    {
//        long mask = 0;
//        var current = roleId;
//        var guard = 0;
//        while (guard++ < 32)
//        {
//            mask |= await GetRoleMaskAsync(current, cancellationToken);
//            var parentId = await _context.Set<Role1>()
//                .AsNoTracking()
//                .Where(r => r.Id == current)
//                .Select(r => r.ParentRoleId)
//                .FirstOrDefaultAsync(cancellationToken);

//            if (!parentId.HasValue || parentId.Value == current)
//                break;

//            current = parentId.Value;
//        }

//        return mask;
//    }

//    private async Task InvalidateUserMaskCacheByRoleAsync(int roleId, CancellationToken cancellationToken)
//    {
//        var userIds = await _context.Users
//            .AsNoTracking()
//            .Where(u => u.Roleid == roleId)
//            .Select(u => u.IdUser)
//            .ToListAsync(cancellationToken);

//        foreach (var userId in userIds)
//            await _cache.RemoveAsync(GetUserMaskKey(userId), cancellationToken);
//    }

//    private static string GetRoleMaskKey(int roleId) => $"rbac:role_mask:{roleId}";
//    private static string GetUserMaskKey(int userId) => $"rbac:user_mask:{userId}";
//}
