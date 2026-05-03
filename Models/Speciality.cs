using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Speciality
{
    public int IdSpeciality { get; set; }

    public int? Code { get; set; }

    public string? Name { get; set; }

    public int? BranchId { get; set; }

    public int? DepartmentId { get; set; }

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public int? LicensedVolume { get; set; }

    public string? Description { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();

    public virtual ICollection<RatingCalculationTime> RatingCalculationTimes { get; set; } = new List<RatingCalculationTime>();
}
