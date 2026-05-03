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

    public int? IdEducationalProgram { get; set; }

    public int? IdStudyForm { get; set; }

    public BitArray IsAccelerated { get; set; } = null!;

    public DateOnly? Admissionyear { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual EducationalDegree? Degree { get; set; }

    public virtual EducationalProgram? IdEducationalProgramNavigation { get; set; }

    public virtual StudyForm? IdStudyFormNavigation { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
