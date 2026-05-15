using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AccountingJournal
{
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateOnly? RealBackTime { get; set; }

    public string? Comment { get; set; }

    public Guid StudentId { get; set; }

    public Guid? InventorySgid { get; set; }

    public bool IsBack { get; set; }

    public Guid IdAccountingJournal { get; set; }

    public virtual InventorySg? InventorySg { get; set; }

    public virtual Student Student { get; set; } = null!;
}
