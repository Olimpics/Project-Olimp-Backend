using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AddDiscipline
{
    public int IdAddDisciplines { get; set; }

    public string? NameAddDisciplines { get; set; }

    public string? CodeAddDisciplines { get; set; }

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
    public virtual AddDetail? AddDetail { get; set; } = new AddDetail();

    public virtual ICollection<BindAddDiscipline> BindAddDisciplines { get; set; } = new List<BindAddDiscipline>();

    public virtual ICollection<BindLoansMain> BindLoansMains { get; set; } = new List<BindLoansMain>();

    public virtual EducationalDegree? DegreeLevel { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual CatalogYearsSelective? IdCatalogNavigation { get; set; }

    public virtual ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

    public virtual TypeOfDiscipline? Type { get; set; }
}
