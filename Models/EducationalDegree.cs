using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalDegree
{
    public int IdEducationalDegree { get; set; }

    public string NameEducationalDegreec { get; set; } = null!;

    public virtual ICollection<AddDiscipline> AddDisciplines { get; set; } = new List<AddDiscipline>();

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
