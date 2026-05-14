using System.Collections;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IDeviceTransferService
{
    Task<(CreateTransferSessionResponse? Response, string? Error)> CreateTransferSessionAsync(Guid userId, CreateTransferSessionRequest request);
    Task<(bool Valid, string? Error)> ValidateTransferCodeAsync(string code);
    Task<(JoinTransferSessionResponse? Response, string? Error)> JoinTransferSessionAsync(Guid userId, JoinTransferSessionRequest request);
    Task<(bool Success, string? Error)> UploadEncryptedPayloadAsync(Guid userId, UploadTransferPayloadRequest request);
    Task<(TransferPayloadResponse? Response, string? Error)> DownloadEncryptedPayloadAsync(Guid userId, Guid sessionToken);
    Task<(bool Success, string? Error)> CompleteTransferAsync(Guid userId, Guid sessionToken);
    Task<(TransferSessionStatusResponse? Response, string? Error)> GetTransferStatusAsync(Guid userId, Guid sessionToken);
}

public class DeviceTransferService : IDeviceTransferService
{
    private readonly IDeviceTransferRepository _repository;
    private readonly IMapper _mapper;
    private const int TransferCodeExpirationMinutes = 15;

    public DeviceTransferService(IDeviceTransferRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(CreateTransferSessionResponse? Response, string? Error)> CreateTransferSessionAsync(Guid userId, CreateTransferSessionRequest request)
    {
        // PROTECTION: Unauthorized device linking
        // Ensure the old device actually belongs to the user requesting the transfer
        if (!await _repository.IsDeviceOwnedByUserAsync(request.OldDeviceId, userId))
            return (null, "Unauthorized device access");

        var session = new DeviceTransferSession
        {
            IdDeviceTransferSessions = Guid.NewGuid(),
            UserId = userId,
            TransferCode = GenerateTransferCode(),
            TransferSessionToken = Guid.NewGuid(),
            OldDeviceId = request.OldDeviceId,
            OldDevicePublicKey = request.OldDevicePublicKey,
            ExpiresAt = DateTime.UtcNow.AddMinutes(TransferCodeExpirationMinutes),
            CreatedAt = DateTime.UtcNow,
            IsCompleted = new BitArray(new[] { false }),
            IsExpired = new BitArray(new[] { false })
        };

        await _repository.CreateTransferSessionAsync(session);

        return (new CreateTransferSessionResponse
        {
            TransferCode = session.TransferCode,
            TransferSessionToken = session.TransferSessionToken,
            ExpiresAt = session.ExpiresAt
        }, null);
    }

    public async Task<(bool Valid, string? Error)> ValidateTransferCodeAsync(string code)
    {
        var session = await _repository.GetByTransferCodeAsync(code);
        
        // PROTECTION: Brute force transfer codes
        // If session not found or expired, return invalid. 
        // Note: Real production should have rate limiting here (handled by middleware).
        if (session == null)
            return (false, "Invalid transfer code");

        if (session.IsExpired.Get(0) || session.ExpiresAt < DateTime.UtcNow)
            return (false, "Transfer code expired");

        if (session.IsCompleted.Get(0))
            return (false, "Transfer already completed");

        return (true, null);
    }

    public async Task<(JoinTransferSessionResponse? Response, string? Error)> JoinTransferSessionAsync(Guid userId, JoinTransferSessionRequest request)
    {
        // PROTECTION: Unauthorized device linking
        // Ensure the new device belongs to the user
        if (!await _repository.IsDeviceOwnedByUserAsync(request.NewDeviceId, userId))
            return (null, "Unauthorized device access");

        var session = await _repository.GetByTransferCodeAsync(request.TransferCode);
        
        if (session == null)
            return (null, "Invalid transfer code");

        // PROTECTION: Cross-user transfer attempts
        // Ensure the transfer is happening within the same user account
        if (session.UserId != userId)
            return (null, "Cross-user transfer forbidden");

        if (session.IsExpired.Get(0) || session.ExpiresAt < DateTime.UtcNow)
            return (null, "Transfer code expired");

        if (session.IsCompleted.Get(0))
            return (null, "Transfer already completed");

        await _repository.AttachNewDeviceAsync(session.TransferSessionToken, request.NewDeviceId, request.NewDevicePublicKey);

        return (new JoinTransferSessionResponse
        {
            TransferSessionToken = session.TransferSessionToken,
            OldDevicePublicKey = session.OldDevicePublicKey
        }, null);
    }

    public async Task<(bool Success, string? Error)> UploadEncryptedPayloadAsync(Guid userId, UploadTransferPayloadRequest request)
    {
        var session = await _repository.GetBySessionTokenAsync(request.TransferSessionToken);
        
        if (session == null || session.UserId != userId)
            return (false, "Transfer session not found or unauthorized");

        // PROTECTION: Expired transfers
        if (session.IsExpired.Get(0) || session.ExpiresAt < DateTime.UtcNow)
            return (false, "Transfer session expired");

        // PROTECTION: Duplicate transfer usage
        if (session.IsCompleted.Get(0))
            return (false, "Transfer already completed");

        await _repository.UploadEncryptedPayloadAsync(request.TransferSessionToken, request.EncryptedTransferPayload);
        
        return (true, null);
    }

    public async Task<(TransferPayloadResponse? Response, string? Error)> DownloadEncryptedPayloadAsync(Guid userId, Guid sessionToken)
    {
        var session = await _repository.GetBySessionTokenAsync(sessionToken);
        
        if (session == null || session.UserId != userId)
            return (null, "Transfer session not found or unauthorized");

        if (string.IsNullOrEmpty(session.EncryptedTransferPayload))
            return (null, "Transfer payload not yet available");

        if (session.IsExpired.Get(0) || session.ExpiresAt < DateTime.UtcNow)
            return (null, "Transfer session expired");

        return (new TransferPayloadResponse
        {
            EncryptedTransferPayload = session.EncryptedTransferPayload,
            OldDevicePublicKey = session.OldDevicePublicKey
        }, null);
    }

    public async Task<(bool Success, string? Error)> CompleteTransferAsync(Guid userId, Guid sessionToken)
    {
        var session = await _repository.GetBySessionTokenAsync(sessionToken);
        
        if (session == null || session.UserId != userId)
            return (false, "Transfer session not found or unauthorized");

        await _repository.MarkCompletedAsync(sessionToken);
        
        return (true, null);
    }

    public async Task<(TransferSessionStatusResponse? Response, string? Error)> GetTransferStatusAsync(Guid userId, Guid sessionToken)
    {
        var session = await _repository.GetBySessionTokenAsync(sessionToken);
        
        if (session == null || session.UserId != userId)
            return (null, "Transfer session not found or unauthorized");

        return (new TransferSessionStatusResponse
        {
            IsCompleted = session.IsCompleted.Get(0),
            IsExpired = session.IsExpired.Get(0) || session.ExpiresAt < DateTime.UtcNow,
            HasPayload = !string.IsNullOrEmpty(session.EncryptedTransferPayload),
            HasNewDevice = session.NewDeviceId.HasValue
        }, null);
    }

    private string GenerateTransferCode()
    {
        // High entropy random code generation
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[8];
        rng.GetBytes(bytes);
        
        var result = new StringBuilder(8);
        foreach (var b in bytes)
        {
            result.Append(chars[b % chars.Length]);
        }
        return result.ToString();
    }
}
