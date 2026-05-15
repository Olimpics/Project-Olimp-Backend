using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Permission
{
    public string Code { get; set; } = null!;

    public int BitIndex { get; set; }

    public Guid IdPermission { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
