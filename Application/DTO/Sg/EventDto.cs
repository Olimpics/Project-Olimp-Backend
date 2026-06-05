using System;

namespace OlimpBack.Application.DTO.Sg;

public class EventDto
{
    public Guid IdEvent { get; set; }
    public string NameEvent { get; set; } = null!;
    public DateOnly Date { get; set; }
    public string Location { get; set; } = null!;
    public string? Format { get; set; }
    public Guid CreatorId { get; set; }
    public Guid SubdivisionSgid { get; set; }
    public Guid RegulationId { get; set; }
    public bool Avail { get; set; }
    public Guid FacultyId { get; set; }
    public Guid CatalogYearId { get; set; }
    public bool IsEven { get; set; }
}

public class EventCreateUpdateDto
{
    public string NameEvent { get; set; } = null!;
    public DateOnly Date { get; set; }
    public string Location { get; set; } = null!;
    public string? Format { get; set; }
    public Guid SubdivisionSgid { get; set; }
    public Guid RegulationId { get; set; }
    public bool Avail { get; set; }
    public Guid FacultyId { get; set; }
    public Guid CatalogYearId { get; set; }
    public bool IsEven { get; set; }
}
