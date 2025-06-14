using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Event
{
    public int IdEvent { get; set; }

    public string NameEvent { get; set; } = null!;

    public DateOnly? Date { get; set; }

    public string? Location { get; set; }

    public string? LinkToRegistration { get; set; }

    public string? Description { get; set; }

    public int? AmountPeople { get; set; }

    public int RegulationId { get; set; }

    public virtual ICollection<BindEvent> BindEvents { get; set; } = new List<BindEvent>();

    public virtual RegulationOnAddPoint Regulation { get; set; } = null!;
}
