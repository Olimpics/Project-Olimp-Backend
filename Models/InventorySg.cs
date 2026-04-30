using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class InventorySg
{
    public int IdInventoroy { get; set; }

    public string? NameInventory { get; set; }

    public string? CodeInventory { get; set; }

    public int? StudentId { get; set; }

    public virtual ICollection<AccountingJournal> AccountingJournals { get; set; } = new List<AccountingJournal>();

    public virtual Student? Student { get; set; }
}
