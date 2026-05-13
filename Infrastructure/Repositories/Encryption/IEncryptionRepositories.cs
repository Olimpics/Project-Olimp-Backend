using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Repositories.Encryption;

public interface IUserDeviceRepository
{
    Task<UserDevice> RegisterDeviceAsync(UserDevice device);
    Task<UserDevice?> GetDeviceByIdAsync(Guid deviceId);
    Task<IEnumerable<UserDevice>> GetUserDevicesAsync(Guid userId);
    Task UpdateLastSeenAsync(Guid deviceId);
    Task UpdateSignedPreKeyAsync(Guid deviceId, string publicKey, string signature, int keyId);
    Task<bool> DeleteDeviceAsync(Guid deviceId, Guid userId);
}

public interface IPreKeyRepository
{
    Task UploadOneTimePreKeysAsync(IEnumerable<PreKey> preKeys);
    Task<PreKey?> GetAndMarkUsedOneTimePreKeyAsync(Guid deviceId);
    Task<int> GetRemainingOneTimePreKeysCountAsync(Guid deviceId);
    Task CleanupUsedPreKeysAsync(DateTime before);
}
