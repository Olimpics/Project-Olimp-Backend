using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SelectiveDetail
{
    public int IdSelectiveDetails { get; set; }

    public string? Recomend { get; set; }

    public int? DepartmentId { get; set; }

    public string? Teachers { get; set; }

    public string? Language { get; set; }

    public string? Prerequisites { get; set; }

    public string? WhyInterestingDetermination { get; set; }

    public string? Provision { get; set; }

    public string? UsingIrl { get; set; }

    public string? ResultEducation { get; set; }

    public string? DisciplineTopics { get; set; }

    public string? TypesOfTraining { get; set; }

    public int? TypeOfControl { get; set; }

    public virtual Department? Department { get; set; }

    public virtual SelectiveDiscipline IdSelectiveDetailsNavigation { get; set; } = null!;

    public virtual TypeOfControl? TypeOfControlNavigation { get; set; }
}
