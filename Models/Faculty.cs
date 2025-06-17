using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Faculty
{
    public int IdFaculty { get; set; }

    public string? NameFaculty { get; set; }

    public string? Abbreviation { get; set; }

    public int? AdminId { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<AddDiscipline> AddDisciplines { get; set; } = new List<AddDiscipline>();

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
