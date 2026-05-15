using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Notification
{
    public string? CustomMessage { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public string? Metadata { get; set; }

    public Guid UserId { get; set; }

    public Guid IdNotification { get; set; }

    public Guid? TemplateId { get; set; }

    public bool IsRead { get; set; }

    public virtual NotificationTemplate? Template { get; set; }

    public virtual User? User { get; set; }
}
