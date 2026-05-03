using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Permission
{
    public int IdPermission { get; set; }

    public string Code { get; set; } = null!;

    public int BitIndex { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
