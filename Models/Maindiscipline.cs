using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Maindiscipline
{
    public int IdBindMainDisciplines { get; set; }

    public string? CodeMainDisciplines { get; set; }

    public string? NameBindMainDisciplines { get; set; }

    public int? Loans { get; set; }

    public string? FormControll { get; set; }

    public int? Semestr { get; set; }

    public int? EducationalProgramId { get; set; }

    public string? Teachers { get; set; }

    public int? Hours { get; set; }

    public int? Idcataloog { get; set; }

    public virtual Educationalprogram? EducationalProgram { get; set; }

    public virtual CatalogyearsMain? IdcataloogNavigation { get; set; }

    public virtual ICollection<Maingrade> Maingrades { get; set; } = new List<Maingrade>();
}
