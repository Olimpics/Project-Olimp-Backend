using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalProgram
{
    public int IdEducationalProgram { get; set; }

    public string? NameEducationalProgram { get; set; }

    public int? CountAddSemestr3 { get; set; }

    public int? CountAddSemestr4 { get; set; }

    public int? CountAddSemestr5 { get; set; }

    public int? CountAddSemestr6 { get; set; }

    public int? CountAddSemestr7 { get; set; }

    public int? CountAddSemestr8 { get; set; }

    public int? DegreeId { get; set; }

    public string? SpecialityCode { get; set; }

    public string? Speciality { get; set; }

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public int? StudentsAmount { get; set; }

    public int? SpecialityId { get; set; }

    public virtual Speciality? SpecialityEntity { get; set; }

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual EducationalDegree? Degree { get; set; }

    public virtual ICollection<MainDiscipline> MainDisciplines { get; set; } = new List<MainDiscipline>();

    public virtual ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
