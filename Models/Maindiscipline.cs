using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainDiscipline
{
    public int IdMainDisciplines { get; set; }

    public string? CodeMainDisciplines { get; set; }

    public string? NameMainDisciplines { get; set; }

    public int? Loans { get; set; }

    public string? FormControll { get; set; }

    public int? Semestr { get; set; }

    public int? EducationalProgramId { get; set; }

    public string? Teachers { get; set; }

    public int? Hours { get; set; }

    public int? Idcataloog { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual CatalogYearsMain? IdcataloogNavigation { get; set; }

    public virtual ICollection<MainGrade> MainGrades { get; set; } = new List<MainGrade>();
}
