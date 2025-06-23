using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AddDetail
{
    public int IdAddDetails { get; set; }

    public string? Recomend { get; set; }

    public int? DepartmentId { get; set; }

    public string? Teachers { get; set; }

    public string? Language { get; set; }

    public string? Prerequisites { get; set; }

    public string? WhyInterestingDetermination { get; set; }

    public string? Determination { get; set; }

    public string? UsingIrl { get; set; }

    public string? ResultEducation { get; set; }

    public string? AdditionaLiterature { get; set; }

    public string TypesOfTraining { get; set; } = null!;

    public string TypeOfControll { get; set; } = null!;

    public virtual Department? Department { get; set; }

    public virtual AddDiscipline IdAddDetailsNavigation { get; set; } = null!;
}
