using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainGrade
{
    public int IdMainGrade { get; set; }

    public int? StudentId { get; set; }

    public int? MainDisciplinesId { get; set; }

    public string? MainGrade1 { get; set; }

    public virtual MainDiscipline? MainDisciplines { get; set; }

    public virtual Student? Student { get; set; }
}
