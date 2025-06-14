using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RegulationOnAddPoint
{
    public int IdRegulationOnAddPoints { get; set; }

    public string CodeRegulationOnAddPoints { get; set; } = null!;

    public string TypeOfActivitys { get; set; } = null!;

    public int? AmountMin { get; set; }

    public int AmountMax { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<BindExtraActivity> BindExtraActivities { get; set; } = new List<BindExtraActivity>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
