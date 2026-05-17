using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainDiscipline
{
    public string CodeMainDisciplines { get; set; } = null!;

    public string NameMainDisciplines { get; set; } = null!;

    public int? Loans { get; set; }

    public string FormControl { get; set; } = null!;

    public int Semestr { get; set; }

    public int? Hours { get; set; }

    public string NameDock { get; set; } = null!;

    public Guid EducationalProgramId { get; set; }

    public bool NeedFix { get; set; }

    public Guid IdMainDisciplines { get; set; }

    public virtual ICollection<BindMainDiscipline> BindMainDisciplines { get; set; } = new List<BindMainDiscipline>();

    public virtual ICollection<BindTeacherMain> BindTeacherMains { get; set; } = new List<BindTeacherMain>();

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;
}
