namespace OlimpBack.Application.Permissions;

public interface IHierarchyAuthorizationService
{
    Task<int> GetUserMaxManagementWeightAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> CanGrantPermissionAsync(
        Guid granterUserId,
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task<bool> CanManageScopeAsync(
        Guid granterUserId,
        Guid? facultyId,
        Guid? departmentId,
        Guid? groupId,
        CancellationToken cancellationToken = default);

    Task<bool> CanAssignHierarchyNodeAsync(
        Guid granterUserId,
        Guid targetHierarchyNodeId,
        Guid? facultyId,
        Guid? departmentId,
        Guid? groupId,
        CancellationToken cancellationToken = default);
}
