using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AddDiscipline
{
    public int IdAddDisciplines { get; set; }

    public string NameAddDisciplines { get; set; } = null!;

    public string CodeAddDisciplines { get; set; } = null!;

    public string Faculty { get; set; } = null!;

    public string Department { get; set; } = null!;

    public int? MinCountPeople { get; set; }

    public int? MaxCountPeople { get; set; }

    public int? MinCourse { get; set; }

    public int? MaxCourse { get; set; }

    public sbyte? AddSemestr { get; set; }

    public string? Recomend { get; set; }

    public string? Teacher { get; set; }

    public string? Prerequisites { get; set; }

    public int? DegreeLevelId { get; set; }

    public virtual ICollection<BindAddDiscipline> BindAddDisciplines { get; set; } = new List<BindAddDiscipline>();

    public virtual EducationalDegree EducationalDegree { get; set; } = null!;
}
