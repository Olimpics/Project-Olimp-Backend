using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO.Encryption;

namespace OlimpBack.Application.Services.Encryption;

public interface IEncryptionSessionService
{
    Task<DeviceKeyResponse> RegisterDeviceKeysAsync(DeviceKeyUploadRequest request, Guid userId);
    Task UploadOneTimePreKeysAsync(Guid deviceId, UploadPreKeysRequest request, Guid userId);
    Task<KeyBundleResponse?> GetRecipientKeyBundleAsync(Guid userId, Guid? deviceId);
    Task<IEnumerable<DeviceKeyResponse>> GetUserDevicesAsync(Guid userId);
    Task<bool> RotateSignedPreKeyAsync(Guid deviceId, KeyRotationRequest request, Guid userId);
}

public interface IKeyManagementService
{
    Task<KeyBundleResponse?> FetchKeyBundleAsync(Guid deviceId);
    Task<int> GetPreKeyStatusAsync(Guid deviceId);
}
