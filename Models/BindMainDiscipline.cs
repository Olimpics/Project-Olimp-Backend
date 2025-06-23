using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindMainDiscipline
{
    public int IdBindMainDisciplines { get; set; }

    public string CodeMainDisciplines { get; set; } = null!;

    public string NameBindMainDisciplines { get; set; } = null!;

    public int Loans { get; set; }

    public string? FormControll { get; set; }

    public int Semestr { get; set; }

    public int EducationalProgramId { get; set; }

    public string? Teachers { get; set; }

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;

    public virtual ICollection<MainGrade> MainGrades { get; set; } = new List<MainGrade>();
}
