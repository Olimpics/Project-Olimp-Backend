using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DisciplineChoicePeriod
{
    public int IdDisciplineChoicePeriod { get; set; }

    /// <summary>
    /// Для всіх = 0, Перевибір = 1
    /// </summary>
    public sbyte PeriodType { get; set; }

    public sbyte PeriodCourse { get; set; }

    public int DegreeLevelId { get; set; }

    public sbyte IsClose { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual EducationalDegree DegreeLevel { get; set; } = null!;

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }
}
