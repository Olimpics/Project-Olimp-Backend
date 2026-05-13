using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.Application.DTO.Encryption;
using OlimpBack.Infrastructure.Repositories.Encryption;
using OlimpBack.Models;

namespace OlimpBack.Application.Services.Encryption;

public class EncryptionSessionService : IEncryptionSessionService
{
    private readonly IUserDeviceRepository _deviceRepository;
    private readonly IPreKeyRepository _preKeyRepository;
    private readonly IMapper _mapper;

    public EncryptionSessionService(
        IUserDeviceRepository deviceRepository,
        IPreKeyRepository preKeyRepository,
        IMapper mapper)
    {
        _deviceRepository = deviceRepository;
        _preKeyRepository = preKeyRepository;
        _mapper = mapper;
    }

    public async Task<DeviceKeyResponse> RegisterDeviceKeysAsync(DeviceKeyUploadRequest request, Guid userId)
    {
        var deviceId = Guid.NewGuid();
        var device = new UserDevice
        {
            IdUserDevices = deviceId,
            UserId = userId,
            DeviceName = request.DeviceName,
            IdentityKey = request.IdentityKey,
            SignedPreKey = request.SignedPreKey.PublicKey,
            SignedPreKeySignature = request.SignedPreKey.Signature,
            SignedPreKeyId = request.SignedPreKey.KeyId,
            CreatedAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

        var preKeys = request.OneTimePreKeys.Select(pk => new PreKey
        {
            IdPreKeys = Guid.NewGuid(),
            DeviceId = deviceId,
            PublicPreKey = pk.PublicKey,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        });

        await _deviceRepository.RegisterDeviceAsync(device);
        if (preKeys.Any())
        {
            await _preKeyRepository.UploadOneTimePreKeysAsync(preKeys);
        }

        return new DeviceKeyResponse { DeviceId = deviceId, IdentityKey = request.IdentityKey };
    }

    public async Task UploadOneTimePreKeysAsync(Guid deviceId, UploadPreKeysRequest request, Guid userId)
    {
        var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
        if (device == null || device.UserId != userId)
            throw new UnauthorizedAccessException("Device not found or access denied.");

        var preKeys = request.OneTimePreKeys.Select(pk => new PreKey
        {
            IdPreKeys = Guid.NewGuid(),
            DeviceId = deviceId,
            PublicPreKey = pk.PublicKey,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        });

        await _preKeyRepository.UploadOneTimePreKeysAsync(preKeys);
    }

    public async Task<KeyBundleResponse?> GetRecipientKeyBundleAsync(Guid userId, Guid? deviceId)
    {
        UserDevice? device;

        if (deviceId.HasValue)
        {
            device = await _deviceRepository.GetDeviceByIdAsync(deviceId.Value);
            if (device == null || device.UserId != userId) return null;
        }
        else
        {
            var devices = await _deviceRepository.GetUserDevicesAsync(userId);
            device = devices.FirstOrDefault();
            if (device == null) return null;
        }

        var otpk = await _preKeyRepository.GetAndMarkUsedOneTimePreKeyAsync(device.IdUserDevices);

        return new KeyBundleResponse
        {
            UserId = device.UserId,
            DeviceId = device.IdUserDevices,
            IdentityKey = device.IdentityKey,
            SignedPreKey = new SignedPreKeyDto
            {
                KeyId = device.SignedPreKeyId,
                PublicKey = device.SignedPreKey,
                Signature = device.SignedPreKeySignature
            },
            OneTimePreKey = otpk != null ? new PreKeyDto { Id = otpk.IdPreKeys, PublicKey = otpk.PublicPreKey } : null
        };
    }

    public async Task<IEnumerable<DeviceKeyResponse>> GetUserDevicesAsync(Guid userId)
    {
        var devices = await _deviceRepository.GetUserDevicesAsync(userId);
        return devices.Select(d => new DeviceKeyResponse { DeviceId = d.IdUserDevices, IdentityKey = d.IdentityKey });
    }

    public async Task<bool> RotateSignedPreKeyAsync(Guid deviceId, KeyRotationRequest request, Guid userId)
    {
        var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
        if (device == null || device.UserId != userId) return false;

        await _deviceRepository.UpdateSignedPreKeyAsync(
            deviceId,
            request.NewSignedPreKey.PublicKey,
            request.NewSignedPreKey.Signature,
            request.NewSignedPreKey.KeyId);

        return true;
    }
}
