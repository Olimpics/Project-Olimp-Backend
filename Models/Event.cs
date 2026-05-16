using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Event
{
    public string NameEvent { get; set; } = null!;

    public DateOnly Date { get; set; }

    public string Location { get; set; } = null!;

    public string? Format { get; set; }

    public Guid CreatorId { get; set; }

    public Guid IdEvent { get; set; }

    public Guid SubdivisionSgid { get; set; }

    public Guid RegulationId { get; set; }

    public bool Avail { get; set; }

    public virtual ICollection<BindEventStudent> BindEventStudents { get; set; } = new List<BindEventStudent>();

    public virtual User Creator { get; set; } = null!;

    public virtual RegulationOnAddPoint Regulation { get; set; } = null!;

    public virtual SubDivisionsSg SubdivisionSg { get; set; } = null!;
}
