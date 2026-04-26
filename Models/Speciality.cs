using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Speciality
{
    public int? IdSpeciality { get; set; }

    public int? Code { get; set; }

    public string? Name { get; set; }

    public int? IdBranch { get; set; }

    public string? IdFaculty { get; set; }

    public string? IdDepartment { get; set; }

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public int? LicensedVolume { get; set; }

    public string? Description { get; set; }
}
