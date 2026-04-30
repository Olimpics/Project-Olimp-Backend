using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RoleInEvent
{
    public int IdRoleInEvent { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<BindEventStudent> BindEventStudents { get; set; } = new List<BindEventStudent>();
}
