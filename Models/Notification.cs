using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Notification
{
    public string CustomMessage { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public string? Metadata { get; set; }

    public Guid UserId { get; set; }

    public Guid IdNotification { get; set; }

    public Guid TemplateId { get; set; }

    public bool IsRead { get; set; }

    public Guid CreatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual NotificationTemplate Template { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
