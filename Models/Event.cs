using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Event
{
    public int IdEvent { get; set; }

    public string? NameEvent { get; set; }

    public DateOnly? Date { get; set; }

    public string? Location { get; set; }

    public int? RegulationId { get; set; }

    public int? SubdivisionSgid { get; set; }

    public string? Format { get; set; }

    public BitArray? Avail { get; set; }

    public Guid? CreatorId { get; set; }

    public virtual ICollection<BindEventStudent> BindEventStudents { get; set; } = new List<BindEventStudent>();

    public virtual User? Creator { get; set; }

    public virtual RegulationOnAddPoint? Regulation { get; set; }

    public virtual SubDivisionsSg? SubdivisionSg { get; set; }
}
