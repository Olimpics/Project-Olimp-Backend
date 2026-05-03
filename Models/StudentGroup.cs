using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class StudentGroup
{
    public int IdGroup { get; set; }

    public string? GroupCode { get; set; }

    public int? NumberOfStudents { get; set; }

    public int? AdminId { get; set; }

    public int? DegreeId { get; set; }

    public int? Course { get; set; }

    public int? EducationalProgramId { get; set; }

    public int? StudyFormId { get; set; }

    public BitArray IsAccelerated { get; set; } = null!;

    public DateOnly? AdmissionYear { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual EducationalDegree? Degree { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual StudyForm? StudyForm { get; set; }
}
