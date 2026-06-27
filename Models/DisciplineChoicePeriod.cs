using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DisciplineChoicePeriod
{
    public bool PeriodType { get; set; }

    public int PeriodCourse { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateOnly EndOfCheckPeriod { get; set; }

    public bool IsClose { get; set; }

    public Guid IdDisciplineChoicePeriod { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid DegreeLevelId { get; set; }

    public bool IsForBothSemester { get; set; }

    public Guid? SpecialityId { get; set; }

    public bool IsShort { get; set; }

    public Guid CatalogYearId { get; set; }

    public Guid? ParentId { get; set; }

    public bool IsConfirm { get; set; }

    public DateTime? ResultsProcessedAt { get; set; }

    public virtual CatalogYear CatalogYear { get; set; } = null!;

    public virtual EducationalDegree DegreeLevel { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<DisciplineChoicePeriod> InverseParent { get; set; } = new List<DisciplineChoicePeriod>();

    public virtual DisciplineChoicePeriod? Parent { get; set; }
}
