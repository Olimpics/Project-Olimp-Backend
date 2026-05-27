using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Permissions;

public interface IPermissionDelegationService
{
    Task<int> GetUserHierarchyLevelAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<int> GetRoleHierarchyLevelAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<bool> CanGrantAsync(Guid granterUserId, Guid targetRoleId, Guid permissionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GrantablePermissionDto>> GetGrantablePermissionsAsync(
        Guid granterUserId,
        Guid targetRoleId,
        CancellationToken cancellationToken = default);
}
