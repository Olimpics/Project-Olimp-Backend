using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Disciplinechoiceperiod
{
    public int? IdDisciplineChoicePeriod { get; set; }

    public int? PeriodType { get; set; }

    public int? PeriodCourse { get; set; }

    public int? DegreeLevelId { get; set; }

    public int? IsClose { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public string? StartDate { get; set; }

    public string? EndDate { get; set; }
}
