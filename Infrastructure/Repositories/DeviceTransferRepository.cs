using System.Collections;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDeviceTransferRepository
{
    Task CreateTransferSessionAsync(DeviceTransferSession session);
    Task<DeviceTransferSession?> GetByTransferCodeAsync(string code);
    Task<DeviceTransferSession?> GetBySessionTokenAsync(Guid token);
    Task AttachNewDeviceAsync(Guid sessionToken, Guid newDeviceId, string newDevicePublicKey);
    Task UploadEncryptedPayloadAsync(Guid sessionToken, string encryptedPayload);
    Task<string?> GetEncryptedPayloadAsync(Guid sessionToken);
    Task MarkCompletedAsync(Guid sessionToken);
    Task ExpireTransferAsync(Guid sessionToken);
    Task CleanupExpiredAsync(DateTime now);
    Task<bool> IsDeviceOwnedByUserAsync(Guid deviceId, Guid userId);
    Task SaveChangesAsync();
}

public class DeviceTransferRepository : IDeviceTransferRepository
{
    private readonly AppDbContext _context;

    public DeviceTransferRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsDeviceOwnedByUserAsync(Guid deviceId, Guid userId)
    {
        return await _context.UserDevices.AnyAsync(d => d.IdUserDevices == deviceId && d.UserId == userId);
    }

    public async Task CreateTransferSessionAsync(DeviceTransferSession session)
    {
        await _context.DeviceTransferSessions.AddAsync(session);
        await SaveChangesAsync();
    }

    public async Task<DeviceTransferSession?> GetByTransferCodeAsync(string code)
    {
        return await _context.DeviceTransferSessions
            .Include(s => s.OldDevice)
            .FirstOrDefaultAsync(s => s.TransferCode == code);
    }

    public async Task<DeviceTransferSession?> GetBySessionTokenAsync(Guid token)
    {
        return await _context.DeviceTransferSessions
            .Include(s => s.OldDevice)
            .Include(s => s.NewDevice)
            .FirstOrDefaultAsync(s => s.TransferSessionToken == token);
    }

    public async Task AttachNewDeviceAsync(Guid sessionToken, Guid newDeviceId, string newDevicePublicKey)
    {
        var session = await _context.DeviceTransferSessions
            .FirstOrDefaultAsync(s => s.TransferSessionToken == sessionToken);
        
        if (session != null)
        {
            session.NewDeviceId = newDeviceId;
            session.NewDevicePublicKey = newDevicePublicKey;
            await SaveChangesAsync();
        }
    }

    public async Task UploadEncryptedPayloadAsync(Guid sessionToken, string encryptedPayload)
    {
        var session = await _context.DeviceTransferSessions
            .FirstOrDefaultAsync(s => s.TransferSessionToken == sessionToken);
            
        if (session != null)
        {
            session.EncryptedTransferPayload = encryptedPayload;
            await SaveChangesAsync();
        }
    }

    public async Task<string?> GetEncryptedPayloadAsync(Guid sessionToken)
    {
        var session = await _context.DeviceTransferSessions
            .FirstOrDefaultAsync(s => s.TransferSessionToken == sessionToken);
            
        return session?.EncryptedTransferPayload;
    }

    public async Task MarkCompletedAsync(Guid sessionToken)
    {
        var session = await _context.DeviceTransferSessions
            .FirstOrDefaultAsync(s => s.TransferSessionToken == sessionToken);
            
        if (session != null)
        {
            session.IsCompleted = true;
            session.CompletedAt = DateTime.UtcNow;
            // Clean up payload immediately after completion for security
            session.EncryptedTransferPayload = null;
            await SaveChangesAsync();
        }
    }

    public async Task ExpireTransferAsync(Guid sessionToken)
    {
        var session = await _context.DeviceTransferSessions
            .FirstOrDefaultAsync(s => s.TransferSessionToken == sessionToken);
            
        if (session != null)
        {
            session.IsExpired = true;
            session.EncryptedTransferPayload = null; // Clear payload on expiry
            await SaveChangesAsync();
        }
    }

    public async Task CleanupExpiredAsync(DateTime now)
    {
        var expiredBit = new BitArray(new[] { true });
        
        // Mark as expired and clear payloads for sessions that passed their expiration date
        await _context.DeviceTransferSessions
            .Where(s => s.ExpiresAt < now && s.IsExpired == false)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.IsExpired, true)
                .SetProperty(p => p.EncryptedTransferPayload, (string?)null));

        // Hard delete sessions that are very old (e.g., more than 24 hours old)
        var hardDeleteThreshold = now.AddHours(-24);
        await _context.DeviceTransferSessions
            .Where(s => s.CreatedAt < hardDeleteThreshold)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
