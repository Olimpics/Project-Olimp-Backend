using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindRoleSystemEvent
{
    public Guid IdBind { get; set; }

    public Guid? RoleId { get; set; }

    public Guid? SystemEventId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual SystemEvent? SystemEvent { get; set; }
}
