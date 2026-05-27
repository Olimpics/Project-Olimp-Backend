namespace OlimpBack.Application.Permissions;

public interface IRoleKindService
{
    /// <summary>User has at least one role with is_student = true.</summary>
    Task<bool> IsStudentAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>User has at least one role with is_admin = true.</summary>
    Task<bool> IsAdminAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Not a student-only user (has staff/admin role).</summary>
    Task<bool> IsStaffAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoleKindSnapshot>> GetUserRoleSnapshotsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<RoleKindSnapshot?> GetRoleSnapshotAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);
}
