using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Speciality
{
    public int IdSpeciality { get; set; }

    public int? Code { get; set; }

    public string? Name { get; set; }

    public int? IdBranch { get; set; }

    public int? IdDepartment { get; set; }

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public int? LicensedVolume { get; set; }

    public string? Description { get; set; }

    public virtual Branch? IdBranchNavigation { get; set; }

    public virtual Department? DepartmentNavigation { get; set; }

    public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
}
