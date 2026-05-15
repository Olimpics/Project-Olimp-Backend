using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindMainDiscipline
{
    public int? Grade { get; set; }

    public int? Semestr { get; set; }

    public Guid StudentId { get; set; }

    public Guid? MainDisciplinesId { get; set; }

    public bool IsRedo { get; set; }

    public Guid? YearId { get; set; }

    public Guid IdBindMainDisciplines { get; set; }

    public virtual MainDiscipline? MainDisciplines { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual CatalogYear? Year { get; set; }
}
