using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DisciplineChoicePeriod
{
    public BitArray? PeriodType { get; set; }

    public int? PeriodCourse { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateOnly? EndOfCheckPeriod { get; set; }

    public bool? IsClose { get; set; }

    public Guid IdDisciplineChoicePeriod { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? DegreeLevelId { get; set; }

    public bool? IsForOnSemestr { get; set; }

    public virtual EducationalDegree? DegreeLevel { get; set; }

    public virtual Department? Department { get; set; }
}
