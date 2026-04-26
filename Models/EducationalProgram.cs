using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Educationalprogram
{
    public int? IdEducationalProgram { get; set; }

    public string? NameEducationalProgram { get; set; }

    public int? CountAddSemestr3 { get; set; }

    public int? CountAddSemestr4 { get; set; }

    public int? CountAddSemestr5 { get; set; }

    public int? CountAddSemestr6 { get; set; }

    public int? CountAddSemestr7 { get; set; }

    public int? CountAddSemestr8 { get; set; }

    public int? DegreeId { get; set; }

    public string? SpecialityCode { get; set; }

    public string? Speciality { get; set; }

    public int? Accreditation { get; set; }

    public string? AccreditationType { get; set; }

    public int? StudentsAmount { get; set; }
}
