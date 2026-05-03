using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class CatalogYear
{
    public int IdCatalog { get; set; }

    public int? YearStart { get; set; }

    public int? YearEnd { get; set; }

    public virtual ICollection<BindRating> BindRatings { get; set; } = new List<BindRating>();

    public virtual ICollection<RatingCalculationTime> RatingCalculationTimes { get; set; } = new List<RatingCalculationTime>();
}
