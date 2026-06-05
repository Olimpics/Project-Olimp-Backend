using System;

namespace OlimpBack.Application.DTO.Sg;

public class InventorySgDto
{
    public Guid IdInventory { get; set; }
    public string InventoryName { get; set; } = null!;
    public string InventoryCode { get; set; } = null!;
    public Guid WatchmanId { get; set; }
    public bool Avail { get; set; }
}

public class InventorySgCreateUpdateDto
{
    public string InventoryName { get; set; } = null!;
    public string InventoryCode { get; set; } = null!;
    public Guid WatchmanId { get; set; }
    public bool Avail { get; set; }
}
