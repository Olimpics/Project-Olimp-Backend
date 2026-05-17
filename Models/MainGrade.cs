using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainGrade
{
    public int MainGradeValue { get; set; }

    public Guid StudentId { get; set; }

    public Guid IdMainGrade { get; set; }

    public Guid MainDisciplinesId { get; set; }

    public virtual MainDiscipline MainDisciplines { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
