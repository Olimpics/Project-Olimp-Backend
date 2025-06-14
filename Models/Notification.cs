using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Notification
{
    public int IdNotification { get; set; }

    public int UserId { get; set; }

    public int? TemplateId { get; set; }

    public string? CustomTitle { get; set; }

    public string? CustomMessage { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public string NotificationType { get; set; } = null!;

    public string? Metadata { get; set; }

    public virtual NotificationTemplate? Template { get; set; }

    public virtual User User { get; set; } = null!;
}
