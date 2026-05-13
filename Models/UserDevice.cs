using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class UserDevice
{
    public Guid IdUserDevices { get; set; }

    public Guid UserId { get; set; }

    public string? DeviceName { get; set; }

    public string PublicKey { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastSeen { get; set; }

    public virtual ICollection<PreKey> PreKeys { get; set; } = new List<PreKey>();

    public virtual User User { get; set; } = null!;
}
