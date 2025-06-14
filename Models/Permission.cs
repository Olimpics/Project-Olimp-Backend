using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Permission
{
    public int IdPermissions { get; set; }

    public string NamePermission { get; set; } = null!;

    public virtual ICollection<BindRolePermission> BindRolePermissions { get; set; } = new List<BindRolePermission>();
}
