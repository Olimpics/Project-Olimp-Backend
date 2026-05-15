using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainGrade
{
    public string? MainGrade1 { get; set; }

    public Guid StudentId { get; set; }

    public Guid IdMainGrade { get; set; }

    public Guid? MainDisciplinesId { get; set; }

    public virtual MainDiscipline? MainDisciplines { get; set; }

    public virtual Student Student { get; set; } = null!;
}
