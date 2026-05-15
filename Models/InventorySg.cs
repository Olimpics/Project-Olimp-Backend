using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class InventorySg
{
    public string? NameInventory { get; set; }

    public string? CodeInventory { get; set; }

    public Guid StudentId { get; set; }

    public bool Avail { get; set; }

    public Guid IdInventoroy { get; set; }

    public virtual ICollection<AccountingJournal> AccountingJournals { get; set; } = new List<AccountingJournal>();

    public virtual Student Student { get; set; } = null!;
}
