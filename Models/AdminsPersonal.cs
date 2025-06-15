using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AdminsPersonal
{
    public int IdAdmins { get; set; }

    public int UserId { get; set; }

    public string NameAdmin { get; set; } = null!;

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public byte[]? Photo { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();

    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual User User { get; set; } = null!;
}
