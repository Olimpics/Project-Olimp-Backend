using System;

namespace OlimpBack.Application.DTO.Sg;

public class AccountingJournalDto
{
    public Guid IdAccountingJournal { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly? RealBackTime { get; set; }
    public string? Comment { get; set; }
    public Guid StudentId { get; set; }
    public Guid InventorySgid { get; set; }
    public bool IsBack { get; set; }
}

public class AccountingJournalCreateUpdateDto
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Comment { get; set; }
    public Guid StudentId { get; set; }
    public Guid InventorySgid { get; set; }
}
