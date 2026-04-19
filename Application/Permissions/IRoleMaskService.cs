namespace OlimpBack.Application.Permissions;

public interface IRoleMaskService
{
    Task<long> RecalculateRoleMaskAsync(int roleId, CancellationToken cancellationToken = default);
    Task<long> GetRoleMaskAsync(int roleId, CancellationToken cancellationToken = default);
    Task<long> GetUserPermissionsMaskAsync(int userId, CancellationToken cancellationToken = default);
}
