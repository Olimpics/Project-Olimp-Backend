using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalProgram
{
    public int IdEducationalProgram { get; set; }

    public string? NameEducationalProgram { get; set; }

    public int? DegreeId { get; set; }

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public List<int>? SelectiveDisciplineBySemestr { get; set; }

    public int? SpecialityId { get; set; }

    public BitArray? IsSpecialization { get; set; }

    public BitArray? NeedFix { get; set; }

    public string? NameDock { get; set; }

    public virtual ICollection<BindEpspecialization> BindEpspecializations { get; set; } = new List<BindEpspecialization>();

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual EducationalDegree? Degree { get; set; }

    public virtual ICollection<MainDiscipline> MainDisciplines { get; set; } = new List<MainDiscipline>();

    public virtual ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

    public virtual Speciality? Speciality { get; set; }

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
