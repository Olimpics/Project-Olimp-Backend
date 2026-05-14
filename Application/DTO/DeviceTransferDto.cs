using System;
using System.ComponentModel.DataAnnotations;

namespace OlimpBack.Application.DTO;

public class CreateTransferSessionRequest
{
    [Required]
    public Guid OldDeviceId { get; set; }

    [Required]
    public string OldDevicePublicKey { get; set; } = null!;
}

public class CreateTransferSessionResponse
{
    public string TransferCode { get; set; } = null!;
    public Guid TransferSessionToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class JoinTransferSessionRequest
{
    [Required]
    public string TransferCode { get; set; } = null!;

    [Required]
    public Guid NewDeviceId { get; set; }

    [Required]
    public string NewDevicePublicKey { get; set; } = null!;
}

public class JoinTransferSessionResponse
{
    public Guid TransferSessionToken { get; set; }
    public string OldDevicePublicKey { get; set; } = null!;
}

public class UploadTransferPayloadRequest
{
    [Required]
    public Guid TransferSessionToken { get; set; }

    [Required]
    public string EncryptedTransferPayload { get; set; } = null!;
}

public class TransferPayloadResponse
{
    public string EncryptedTransferPayload { get; set; } = null!;
    public string OldDevicePublicKey { get; set; } = null!;
}

public class TransferSessionStatusResponse
{
    public bool IsCompleted { get; set; }
    public bool IsExpired { get; set; }
    public bool HasPayload { get; set; }
    public bool HasNewDevice { get; set; }
}
