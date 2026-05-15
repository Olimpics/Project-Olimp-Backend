using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Approval
{
    public string? AppovalStatus { get; set; }

    public Guid? RoleId { get; set; }

    public Guid IdApproval { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
