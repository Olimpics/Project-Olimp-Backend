using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RatingCalculationTime
{
    public int IdRatingCalculatioTime { get; set; }

    public int? SpecialityId { get; set; }

    public int? Course { get; set; }

    public int? Semestr { get; set; }

    public BitArray? IsShorted { get; set; }

    public DateOnly? Date { get; set; }

    public int? YearId { get; set; }

    public virtual Speciality? Speciality { get; set; }

    public virtual CatalogYear? Year { get; set; }
}
