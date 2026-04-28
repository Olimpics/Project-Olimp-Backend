using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Notification
{
    public int IdNotification { get; set; }

    public int? UserId { get; set; }

    public int? TemplateId { get; set; }

    public string? CustomMessage { get; set; }

    public BitArray? IsRead { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual NotificationTemplate? Template { get; set; }

    public virtual User? User { get; set; }
}
