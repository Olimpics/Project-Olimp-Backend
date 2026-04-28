using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DisciplineChoicePeriod
{
    public int IdDisciplineChoicePeriod { get; set; }

    public BitArray? PeriodType { get; set; }

    public int? PeriodCourse { get; set; }

    public int? DegreeLevelId { get; set; }

    public BitArray? IsClose { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public string? StartDate { get; set; }

    public string? EndDate { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }
}
