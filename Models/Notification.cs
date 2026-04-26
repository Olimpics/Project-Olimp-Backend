using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Notification
{
    public int? IdNotification { get; set; }

    public int? UserId { get; set; }

    public int? TemplateId { get; set; }

    public string? CustomMessage { get; set; }

    public int? IsRead { get; set; }

    public string? CreatedAt { get; set; }

    public string? Metadata { get; set; }
}
