using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Notificationtemplate
{
    public int? IdNotificationTemplates { get; set; }

    public string? NotificationType { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }
}
