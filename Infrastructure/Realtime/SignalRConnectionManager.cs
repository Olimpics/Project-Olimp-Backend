using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Infrastructure.Realtime;

public class SignalRConnectionManager : ISignalRConnectionManager
{
    // userId -> set of connectionIds
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> _userConnections = new();
    
    // connectionId -> userId
    private static readonly ConcurrentDictionary<string, Guid> _connectionUsers = new();
    
    // connectionId -> deviceId
    private static readonly ConcurrentDictionary<string, Guid> _connectionDevices = new();
    
    // deviceId -> set of connectionIds
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> _deviceConnections = new();

    public Task AddConnectionAsync(Guid userId, string connectionId)
    {
        _connectionUsers[connectionId] = userId;
        
        var connections = _userConnections.GetOrAdd(userId, _ => new HashSet<string>());
        lock (connections)
        {
            connections.Add(connectionId);
        }
        
        return Task.CompletedTask;
    }

    public Task RemoveConnectionAsync(string connectionId)
    {
        if (_connectionUsers.TryRemove(connectionId, out var userId))
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(connectionId);
                    if (connections.Count == 0)
                    {
                        _userConnections.TryRemove(userId, out _);
                    }
                }
            }
        }

        if (_connectionDevices.TryRemove(connectionId, out var deviceId))
        {
            if (_deviceConnections.TryGetValue(deviceId, out var devConnections))
            {
                lock (devConnections)
                {
                    devConnections.Remove(connectionId);
                    if (devConnections.Count == 0)
                    {
                        _deviceConnections.TryRemove(deviceId, out _);
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task BindDeviceToConnectionAsync(string connectionId, Guid deviceId)
    {
        _connectionDevices[connectionId] = deviceId;
        
        var connections = _deviceConnections.GetOrAdd(deviceId, _ => new HashSet<string>());
        lock (connections)
        {
            connections.Add(connectionId);
        }
        
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetConnectionsByUserAsync(Guid userId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                return Task.FromResult<IEnumerable<string>>(connections.ToList());
            }
        }
        return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
    }

    public Task<IEnumerable<string>> GetConnectionsByDeviceAsync(Guid deviceId)
    {
        if (_deviceConnections.TryGetValue(deviceId, out var connections))
        {
            lock (connections)
            {
                return Task.FromResult<IEnumerable<string>>(connections.ToList());
            }
        }
        return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
    }

    public Task<Guid?> GetUserIdByConnectionAsync(string connectionId)
    {
        if (_connectionUsers.TryGetValue(connectionId, out var userId))
        {
            return Task.FromResult<Guid?>(userId);
        }
        return Task.FromResult<Guid?>(null);
    }

    public Task<Guid?> GetDeviceIdByConnectionAsync(string connectionId)
    {
        if (_connectionDevices.TryGetValue(connectionId, out var deviceId))
        {
            return Task.FromResult<Guid?>(deviceId);
        }
        return Task.FromResult<Guid?>(null);
    }
}
