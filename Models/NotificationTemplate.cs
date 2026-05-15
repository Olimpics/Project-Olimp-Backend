using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class NotificationTemplate
{
    public string? NotificationType { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public bool Avail { get; set; }

    public Guid IdNotificationTemplates { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
