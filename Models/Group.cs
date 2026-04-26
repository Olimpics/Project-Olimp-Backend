using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Group
{
    public int IdGroup { get; set; }

    public string? GroupCode { get; set; }

    public int? NumberOfStudents { get; set; }

    public int? AdminId { get; set; }

    public int? DegreeId { get; set; }

    public int? Course { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public int? IdEducationalProgram { get; set; }

    public int? IdSpeciality { get; set; }

    public int? AdmissionYear { get; set; }

    public int? IdStudyForm { get; set; }

    public int? IdSpecialization { get; set; }

    public int IsAccelerated { get; set; }
}
