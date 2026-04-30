using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AccountingJournal
{
    public int IdAccountingJournal { get; set; }

    public int? StudentId { get; set; }

    public int? InventorySgid { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public BitArray? IsBack { get; set; }

    public DateOnly? RealBackTime { get; set; }

    public string? Comment { get; set; }

    public virtual InventorySg? InventorySg { get; set; }

    public virtual Student? Student { get; set; }
}
