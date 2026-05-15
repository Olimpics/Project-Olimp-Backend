using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class UserDevice
{
    public Guid IdUserDevices { get; set; }

    public Guid UserId { get; set; }

    public string DeviceName { get; set; } = null!;

    public string IdentityKey { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastSeen { get; set; }

    public string SignedPreKey { get; set; } = null!;

    public string SignedPreKeySignature { get; set; } = null!;

    public int SignedPreKeyId { get; set; }

    public virtual ICollection<DeviceTransferSession> DeviceTransferSessionNewDevices { get; set; } = new List<DeviceTransferSession>();

    public virtual ICollection<DeviceTransferSession> DeviceTransferSessionOldDevices { get; set; } = new List<DeviceTransferSession>();

    public virtual ICollection<PreKey> PreKeys { get; set; } = new List<PreKey>();

    public virtual User User { get; set; } = null!;
}
