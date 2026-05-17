using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SelectiveDetail
{
    public string? Language { get; set; }

    public string? Prerequisites { get; set; }

    public string? WhyInterestingDetermination { get; set; }

    public string? Provision { get; set; }

    public string? UsingIrl { get; set; }

    public string? ResultEducation { get; set; }

    public string? TypesOfTraining { get; set; }

    public string Recommended { get; set; } = null!;

    public string? NameSelectiveDisciplinesEng { get; set; }

    public List<string>? DisciplineTopics { get; set; }

    public string? Teachers { get; set; }

    public Guid IdSelectiveDetails { get; set; }

    public virtual SelectiveDiscipline IdSelectiveDetailsNavigation { get; set; } = null!;
}
