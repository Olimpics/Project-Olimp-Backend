using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RatingCalculationTime
{
    public int Course { get; set; }

    public DateOnly Date { get; set; }

    public Guid YearId { get; set; }

    public bool IsEven { get; set; }

    public bool IsShorted { get; set; }

    public Guid SpecialityId { get; set; }

    public Guid IdRatingCalculationTime { get; set; }

    public virtual Speciality Speciality { get; set; } = null!;

    public virtual CatalogYear Year { get; set; } = null!;
}
