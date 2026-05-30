using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SystemEvent
{
    public Guid IdSystemEvent { get; set; }

    public string? NameSystemEvent { get; set; }

    public string? MessegeSystemEvent { get; set; }

    public virtual ICollection<BindRoleSystemEvent> BindRoleSystemEvents { get; set; } = new List<BindRoleSystemEvent>();
}
