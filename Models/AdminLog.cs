using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AdminLog
{
    public int LogId { get; set; }

    public int? AdminId { get; set; }

    public string? ChangeTime { get; set; }

    public string? TableName { get; set; }

    public string? Action { get; set; }

    public int? KeyValue { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }
}
