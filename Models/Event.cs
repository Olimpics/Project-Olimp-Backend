using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Event
{
    public int IdEvent { get; set; }

    public string? NameEvent { get; set; }

    public DateOnly? Date { get; set; }

    public string? Location { get; set; }

    public int? RegulationId { get; set; }

    public int? CeatorId { get; set; }

    public int? SubdivisionSgid { get; set; }

    public string? Format { get; set; }

    public virtual ICollection<BindEventStudent> BindEventStudents { get; set; } = new List<BindEventStudent>();

    public virtual ICollection<BindEvent> BindEvents { get; set; } = new List<BindEvent>();

    public virtual User? Ceator { get; set; }

    public virtual RegulationOnAddPoint? Regulation { get; set; }

    public virtual SubDivisionsSg? SubdivisionSg { get; set; }
}
