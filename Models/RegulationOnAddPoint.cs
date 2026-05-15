using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RegulationOnAddPoint
{
    public string CodeRegulationOnAddPoints { get; set; } = null!;

    public string TypeOfActivitys { get; set; } = null!;

    public int AmountMin { get; set; }

    public int AmountMax { get; set; }

    public string? Notes { get; set; }

    public string? SubTypeOfActivitys { get; set; }

    public bool Avail { get; set; }

    public Guid IdRegulationOnAddPoints { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
