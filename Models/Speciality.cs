using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Speciality
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public int? LicensedVolume { get; set; }

    public string? Description { get; set; }

    public Guid BranchId { get; set; }

    public Guid DepartmentId { get; set; }

    public bool Avail { get; set; }

    public Guid IdSpeciality { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();

    public virtual ICollection<RatingCalculationTime> RatingCalculationTimes { get; set; } = new List<RatingCalculationTime>();
}
