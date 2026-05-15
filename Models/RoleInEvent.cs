using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RoleInEvent
{
    public string? RoleName { get; set; }

    public Guid IdRoleInEvent { get; set; }

    public virtual ICollection<BindEventStudent> BindEventStudents { get; set; } = new List<BindEventStudent>();
}
