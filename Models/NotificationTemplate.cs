using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class NotificationTemplate
{
    public int IdNotificationTemplates { get; set; }

    public string NotificationType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
