using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Group
{
    public int IdGroup { get; set; }

    public string GroupCode { get; set; } = null!;

    public int? NumberOfStudents { get; set; }

    public int? AdminId { get; set; }

    public int? DegreeId { get; set; }

    public int? Course { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual EducationalDegree? Degree { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
