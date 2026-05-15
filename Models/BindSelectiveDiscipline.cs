using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindSelectiveDiscipline
{
    public int? Loans { get; set; }

    public int? Semestr { get; set; }

    public int? StudentAssessment { get; set; }

    public Guid StudentId { get; set; }

    public Guid? YearId { get; set; }

    public int? Grade { get; set; }

    public bool? IsRedo { get; set; }

    public bool? NeedReview { get; set; }

    public bool? InProcess { get; set; }

    public Guid? SelectiveDisciplineId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid IdBindSelectiveDisciplines { get; set; }

    public virtual SelectiveDiscipline? SelectiveDiscipline { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual CatalogYear? Year { get; set; }
}
