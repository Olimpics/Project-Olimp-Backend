using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MainDiscipline
{
    public int IdBindMainDisciplines { get; set; }

    public string? CodeMainDisciplines { get; set; }

    public string? NameBindMainDisciplines { get; set; }

    public int? Loans { get; set; }

    public string? FormControl { get; set; }

    public int? Semestr { get; set; }

    public int? EducationalProgramId { get; set; }

    public int? Hours { get; set; }

    public int? CatalogId { get; set; }

    public virtual BindMainDiscipline? BindMainDiscipline { get; set; }

    public virtual ICollection<BindTeacherMain> BindTeacherMains { get; set; } = new List<BindTeacherMain>();

    public virtual CatalogYearsMain? Catalog { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual ICollection<MainGrade> MainGrades { get; set; } = new List<MainGrade>();
}
