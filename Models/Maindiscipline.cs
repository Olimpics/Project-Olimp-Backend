using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainDiscipline
{
    public string? CodeMainDisciplines { get; set; }

    public string? NameBindMainDisciplines { get; set; }

    public int? Loans { get; set; }

    public string? FormControl { get; set; }

    public int? Semestr { get; set; }

    public int? Hours { get; set; }

    public string? NameDock { get; set; }

    public Guid? EducationalProgramId { get; set; }

    public bool NeedFix { get; set; }

    public Guid IdMainDisciplines { get; set; }

    public virtual ICollection<BindMainDiscipline> BindMainDisciplines { get; set; } = new List<BindMainDiscipline>();

    public virtual ICollection<BindTeacherMain> BindTeacherMains { get; set; } = new List<BindTeacherMain>();

    public virtual EducationalProgram? EducationalProgram { get; set; }
}
