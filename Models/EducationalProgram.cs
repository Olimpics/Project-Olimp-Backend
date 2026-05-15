using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalProgram
{
    public string? NameEducationalProgram { get; set; }

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public List<int>? SelectiveDisciplineBySemestr { get; set; }

    public string? NameDock { get; set; }

    public List<int>? MinUniSelectiveDisciplineBySemestr { get; set; }

    public string? Subject { get; set; }

    public string? Goals { get; set; }

    public string? TheoreticalContent { get; set; }

    public string? Methodics { get; set; }

    public string? Instrument { get; set; }

    public List<string>? Keys { get; set; }

    public Guid IdEducationalProgram { get; set; }

    public Guid? CatalogId { get; set; }

    public Guid DegreeId { get; set; }

    public Guid? SpecializationId { get; set; }

    public bool? NeedFix { get; set; }

    public Guid? SpecialityId { get; set; }

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual ICollection<BindSimilaEducationalProgramInGroup> BindSimilaEducationalProgramInGroups { get; set; } = new List<BindSimilaEducationalProgramInGroup>();

    public virtual CatalogYearsMain? Catalog { get; set; }

    public virtual EducationalDegree? Degree { get; set; }

    public virtual ICollection<MainDiscipline> MainDisciplines { get; set; } = new List<MainDiscipline>();

    public virtual ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

    public virtual Speciality? Speciality { get; set; }

    public virtual Specialization? Specialization { get; set; }

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
}
