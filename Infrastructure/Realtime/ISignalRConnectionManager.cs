using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlimpBack.Infrastructure.Realtime;

public interface ISignalRConnectionManager
{
    Task AddConnectionAsync(Guid userId, string connectionId);
    Task RemoveConnectionAsync(string connectionId);
    Task BindDeviceToConnectionAsync(string connectionId, Guid deviceId);
    
    Task<IEnumerable<string>> GetConnectionsByUserAsync(Guid userId);
    Task<IEnumerable<string>> GetConnectionsByDeviceAsync(Guid deviceId);
    Task<Guid?> GetUserIdByConnectionAsync(string connectionId);
    Task<Guid?> GetDeviceIdByConnectionAsync(string connectionId);
}
