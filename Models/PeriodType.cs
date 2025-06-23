using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class PeriodType
{
    public int IdPeriodType { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; } = new List<DisciplineChoicePeriod>();
}
