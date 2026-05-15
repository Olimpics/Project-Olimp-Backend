using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SelectiveDiscipline
{
    public string? NameSelectiveDisciplines { get; set; }

    public string? CodeSelectiveDisciplines { get; set; }

    public bool? IsFaculty { get; set; }

    public int? MinCountPeople { get; set; }

    public int? MaxCountPeople { get; set; }

    public int? IsForseChange { get; set; }

    public List<int>? SimilarId { get; set; }

    public string? Feedback { get; set; }

    public string? NameDock { get; set; }

    public List<int>? Courses { get; set; }

    public List<string>? Keys { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? DegreeLevelId { get; set; }

    public Guid? CatalogId { get; set; }

    public Guid? ApprovalStatusId { get; set; }

    public List<Guid>? RecommendedEp { get; set; }

    public bool? NeedFix { get; set; }

    public bool? IsEven { get; set; }

    public Guid IdSelectiveDisciplines { get; set; }

    public Guid? TypeOfControlId { get; set; }

    public Guid? TypeId { get; set; }

    public virtual Approval? ApprovalStatus { get; set; }

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual ICollection<BindSelectiveDiscipline> BindSelectiveDisciplines { get; set; } = new List<BindSelectiveDiscipline>();

    public virtual ICollection<BindSimilarSelectiveInGroup> BindSimilarSelectiveInGroups { get; set; } = new List<BindSimilarSelectiveInGroup>();

    public virtual ICollection<BindTeachersSelective> BindTeachersSelectives { get; set; } = new List<BindTeachersSelective>();

    public virtual CatalogYearsSelective? Catalog { get; set; }

    public virtual EducationalDegree? DegreeLevel { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<GroupSimilarSelective> GroupSimilarSelectives { get; set; } = new List<GroupSimilarSelective>();

    public virtual ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

    public virtual SelectiveDetail? SelectiveDetail { get; set; }

    public virtual TypeOfDiscipline? Type { get; set; }

    public virtual TypeOfControl? TypeOfControl { get; set; }
}
