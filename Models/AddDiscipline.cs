using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AddDiscipline
{
    public int IdAddDisciplines { get; set; }

    public string NameAddDisciplines { get; set; } = null!;

    public string CodeAddDisciplines { get; set; } = null!;

    public sbyte IsFaculty { get; set; }

    public int FacultyId { get; set; }

    public int? MinCountPeople { get; set; }

    public int? MaxCountPeople { get; set; }

    public int? MinCourse { get; set; }

    public int? MaxCourse { get; set; }

    public sbyte? AddSemestr { get; set; }

    public int? DegreeLevelId { get; set; }

    public int? TypeId { get; set; }

    public sbyte IsForseChange { get; set; }

    public virtual AddDetail? AddDetail { get; set; }

    public virtual ICollection<BindAddDiscipline> BindAddDisciplines { get; set; } = new List<BindAddDiscipline>();

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual EducationalDegree? DegreeLevel { get; set; }

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual TypeOfDiscipline? Type { get; set; }
}
