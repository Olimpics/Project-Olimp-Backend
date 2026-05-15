using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DeviceTransferSession
{
    public Guid IdDeviceTransferSessions { get; set; }

    public Guid UserId { get; set; }

    public string TransferCode { get; set; } = null!;

    public Guid TransferSessionToken { get; set; }

    public Guid OldDeviceId { get; set; }

    public Guid? NewDeviceId { get; set; }

    public string OldDevicePublicKey { get; set; } = null!;

    public string? NewDevicePublicKey { get; set; }

    public string? EncryptedTransferPayload { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsExpired { get; set; }

    public virtual UserDevice? NewDevice { get; set; }

    public virtual UserDevice OldDevice { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
