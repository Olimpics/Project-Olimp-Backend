using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AdminLog
{
    public string ChangeTime { get; set; } = null!;

    public string? TableName { get; set; }

    public string Action { get; set; } = null!;

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public Guid AdminId { get; set; }

    public Guid IdAdminLogs { get; set; }

    public Guid KeyValue { get; set; }

    public virtual AdminsPersonal Admin { get; set; } = null!;
}
