using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class InventorySg
{
    public string InventoryName { get; set; } = null!;

    public string InventoryCode { get; set; } = null!;

    public Guid WatchmanId { get; set; }

    public bool Avail { get; set; }

    public Guid IdInventory { get; set; }

    public virtual ICollection<AccountingJournal> AccountingJournals { get; set; } = new List<AccountingJournal>();

    public virtual Student Watchman { get; set; } = null!;
}
