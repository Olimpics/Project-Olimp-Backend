using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OlimpBack.Infrastructure.Repositories.Encryption;

namespace OlimpBack.Infrastructure.Security;

public class DeviceTrustService : IDeviceTrustService
{
    private readonly IUserDeviceRepository _deviceRepository;

    public DeviceTrustService(IUserDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<bool> IsDeviceTrustedAsync(Guid userId, Guid deviceId)
    {
        var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
        return device != null && device.UserId == userId;
    }

    public Task ValidateMessageIntegrityAsync(byte[] payload, string providedHash)
    {
        using var sha256 = SHA256.Create();
        var computedHashBytes = sha256.ComputeHash(payload);
        var computedHash = Convert.ToHexString(computedHashBytes).ToLower();

        if (computedHash != providedHash.ToLower())
        {
            throw new CryptographicException("Message integrity check failed. Hash mismatch.");
        }
        
        return Task.CompletedTask;
    }
}
