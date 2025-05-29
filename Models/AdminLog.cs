using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AdminLog
{
    public long LogId { get; set; }

    /// <summary>
    /// idUsers who performed action
    /// </summary>
    public int AdminId { get; set; }

    public DateTime ChangeTime { get; set; }

    public string TableName { get; set; } = null!;

    public string Action { get; set; } = null!;

    /// <summary>
    /// Primary key of affected row
    /// </summary>
    public string KeyValue { get; set; } = null!;

    public string? OldData { get; set; }

    public string? NewData { get; set; }
}
