using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Permission
{
    public int IdPermissions { get; set; }

    public string TypePermission { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public virtual ICollection<BindRolePermission> BindRolePermissions { get; set; } = new List<BindRolePermission>();
}
