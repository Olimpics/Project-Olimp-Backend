using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainGrade
{
    public int IdMainGrade { get; set; }

    public int StudentId { get; set; }

    public int MainDisciplinesId { get; set; }

    public int? MainGrade1 { get; set; }

    public virtual BindMainDiscipline MainDisciplines { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
