using System;
using System.Threading;
using System.Threading.Tasks;

namespace OlimpBack.Application.Permissions;

public interface IRoleMaskService
{
    Task<long> RecalculateRoleMaskAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<long> GetRoleMaskAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<long> GetUserPermissionsMaskAsync(Guid userId, CancellationToken cancellationToken = default);
}
