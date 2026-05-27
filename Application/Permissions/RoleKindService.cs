using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;

namespace OlimpBack.Application.Permissions;

public class RoleKindService : IRoleKindService
{
    private readonly AppDbContext _context;

    public RoleKindService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsStudentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .AnyAsync(ur => ur.Role.IsStudent, cancellationToken);
    }

    public async Task<bool> IsAdminAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .AnyAsync(ur => ur.Role.IsAdmin, cancellationToken);
    }

    public async Task<bool> IsStaffAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .AnyAsync(ur => !ur.Role.IsStudent, cancellationToken);
    }

    public async Task<IReadOnlyList<RoleKindSnapshot>> GetUserRoleSnapshotsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => new RoleKindSnapshot(ur.Role.IsStudent, ur.Role.IsAdmin, ur.Role.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleKindSnapshot?> GetRoleSnapshotAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(r => r.IdRole == roleId)
            .Select(r => new RoleKindSnapshot(r.IsStudent, r.IsAdmin, r.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
