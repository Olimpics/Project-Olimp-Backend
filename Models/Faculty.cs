using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Faculty
{
    public string NameFaculty { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public bool Avail { get; set; }

    public Guid IdFaculty { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
