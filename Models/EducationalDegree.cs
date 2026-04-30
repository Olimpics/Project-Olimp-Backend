using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalDegree
{
    public int IdEducationalDegree { get; set; }

    public string? NameEducationalDegreec { get; set; }

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();

    public virtual ICollection<Normative> Normatives { get; set; } = new List<Normative>();

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
