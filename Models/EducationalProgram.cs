using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalProgram
{
    public string NameEducationalProgram { get; set; } = null!;

    public int Accreditation { get; set; }

    public string AccreditationType { get; set; } = null!;

    public List<int> SelectiveDisciplineBySemestr { get; set; } = null!;

    public string? NameDock { get; set; }

    public List<int> MinUniSelectiveDisciplineBySemestr { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Goals { get; set; } = null!;

    public string TheoreticalContent { get; set; } = null!;

    public string Methodics { get; set; } = null!;

    public string Instrument { get; set; } = null!;

    public List<string>? Keys { get; set; }

    public Guid IdEducationalProgram { get; set; }

    public Guid CatalogId { get; set; }

    public Guid DegreeId { get; set; }

    public Guid SpecializationId { get; set; }

    public bool NeedFix { get; set; }

    public Guid SpecialityId { get; set; }

    public Guid? StudyFormId { get; set; }

    public string? StudyTurm { get; set; }

    public bool IsAccelerated { get; set; }

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual ICollection<BindSimilaEducationalProgramInGroup> BindSimilaEducationalProgramInGroups { get; set; } = new List<BindSimilaEducationalProgramInGroup>();

    public virtual CatalogYearsMain Catalog { get; set; } = null!;

    public virtual EducationalDegree Degree { get; set; } = null!;

    public virtual ICollection<MainDiscipline> MainDisciplines { get; set; } = new List<MainDiscipline>();

    public virtual ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

    public virtual Speciality Speciality { get; set; } = null!;

    public virtual Specialization Specialization { get; set; } = null!;

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    public virtual StudyForm? StudyForm { get; set; }
}
