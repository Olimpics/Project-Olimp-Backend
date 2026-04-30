using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SelectiveDiscipline
{
    public int IdSelectiveDisciplines { get; set; }

    public int IdAddDisciplines { get => IdSelectiveDisciplines; set => IdSelectiveDisciplines = value; }

    public string? NameSelectiveDisciplines { get; set; }

    public string? NameAddDisciplines { get => NameSelectiveDisciplines; set => NameSelectiveDisciplines = value; }

    public string? CodeSelectiveDisciplines { get; set; }

    public string? CodeAddDisciplines { get => CodeSelectiveDisciplines; set => CodeSelectiveDisciplines = value; }

    public int? IsFaculty { get; set; }

    public int? FacultyId { get; set; }

    public int? MinCountPeople { get; set; }

    public int? MaxCountPeople { get; set; }

    public int? MinCourse { get; set; }

    public int? MaxCourse { get; set; }

    public int? IsEven { get; set; }

    public int? DegreeLevelId { get; set; }

    public int? TypeId { get; set; }

    public int? IsForseChange { get; set; }

    public int? IdCatalog { get; set; }

    public List<int>? Idsimilar { get; set; }

    public int? ApprovalStatusId { get; set; }

    public virtual Approval? ApprovalStatus { get; set; }

    public virtual ICollection<BindAddDiscipline> BindAddDisciplines { get; set; } = new List<BindAddDiscipline>();

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual ICollection<BindTeachersSelective> BindTeachersSelectives { get; set; } = new List<BindTeachersSelective>();

    public virtual EducationalDegree? DegreeLevel { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual CatalogYearsSelective? IdCatalogNavigation { get; set; }

    public virtual ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

    public virtual SelectiveDetail? SelectiveDetail { get; set; }

    public virtual TypeOfDiscipline? Type { get; set; }
}
