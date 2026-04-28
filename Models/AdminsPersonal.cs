using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AdminsPersonal
{
    public int IdAdmins { get; set; }

    public int? UserId { get; set; }

    public string? NameAdmin { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    public virtual User? User { get; set; }
}
