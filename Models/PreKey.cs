using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class PreKey
{
    public Guid IdPreKeys { get; set; }

    public Guid DeviceId { get; set; }

    public string PublicPreKey { get; set; } = null!;

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual UserDevice Device { get; set; } = null!;
}
