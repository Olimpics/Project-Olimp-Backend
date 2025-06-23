using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DisciplineChoicePeriod
{
    public int IdDisciplineChoicePeriod { get; set; }

    public int PeriodType { get; set; }

    public int LevelType { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual PeriodLevel LevelTypeNavigation { get; set; } = null!;

    public virtual PeriodType PeriodTypeNavigation { get; set; } = null!;
}
