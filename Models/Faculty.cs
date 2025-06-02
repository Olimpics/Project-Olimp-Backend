using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Faculty
{
    public int IdFaculty { get; set; }

    public string? NameFaculty { get; set; }

    public string? Abbreviation { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
