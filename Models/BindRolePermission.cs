using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindRolePermission
{
    public int IdBindRolePermission { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
