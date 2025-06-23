using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class PeriodLevel
{
    public int IdPeriodLevel { get; set; }

    public string? LevelName { get; set; }

    public virtual ICollection<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; } = new List<DisciplineChoicePeriod>();
}
