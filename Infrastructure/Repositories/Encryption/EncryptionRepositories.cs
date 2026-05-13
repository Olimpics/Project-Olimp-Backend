using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Repositories.Encryption;

public class UserDeviceRepository : IUserDeviceRepository
{
    private readonly AppDbContext _context;

    public UserDeviceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDevice> RegisterDeviceAsync(UserDevice device)
    {
        _context.UserDevices.Add(device);
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task<UserDevice?> GetDeviceByIdAsync(Guid deviceId)
    {
        return await _context.UserDevices
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.IdUserDevices == deviceId);
    }

    public async Task<IEnumerable<UserDevice>> GetUserDevicesAsync(Guid userId)
    {
        return await _context.UserDevices
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.LastSeen)
            .ToListAsync();
    }

    public async Task UpdateLastSeenAsync(Guid deviceId)
    {
        var device = await _context.UserDevices.FindAsync(deviceId);
        if (device != null)
        {
            device.LastSeen = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateSignedPreKeyAsync(Guid deviceId, string publicKey, string signature, int keyId)
    {
        var device = await _context.UserDevices.FindAsync(deviceId);
        if (device != null)
        {
            device.SignedPreKey = publicKey;
            device.SignedPreKeySignature = signature;
            device.SignedPreKeyId = keyId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> DeleteDeviceAsync(Guid deviceId, Guid userId)
    {
        var device = await _context.UserDevices.FindAsync(deviceId);
        if (device == null || device.UserId != userId) return false;

        _context.UserDevices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }
}

public class PreKeyRepository : IPreKeyRepository
{
    private readonly AppDbContext _context;

    public PreKeyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task UploadOneTimePreKeysAsync(IEnumerable<PreKey> preKeys)
    {
        _context.PreKeys.AddRange(preKeys);
        await _context.SaveChangesAsync();
    }

    public async Task<PreKey?> GetAndMarkUsedOneTimePreKeyAsync(Guid deviceId)
    {
        // Using a transaction/lock would be better for high concurrency
        var preKey = await _context.PreKeys
            .Where(p => p.DeviceId == deviceId && !p.IsUsed)
            .OrderBy(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        if (preKey != null)
        {
            preKey.IsUsed = true;
            await _context.SaveChangesAsync();
        }

        return preKey;
    }

    public async Task<int> GetRemainingOneTimePreKeysCountAsync(Guid deviceId)
    {
        return await _context.PreKeys
            .CountAsync(p => p.DeviceId == deviceId && !p.IsUsed);
    }

    public async Task CleanupUsedPreKeysAsync(DateTime before)
    {
        var usedPreKeys = _context.PreKeys
            .Where(p => p.IsUsed && p.CreatedAt < before);
        
        _context.PreKeys.RemoveRange(usedPreKeys);
        await _context.SaveChangesAsync();
    }
}
