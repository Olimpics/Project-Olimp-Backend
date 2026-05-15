using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RolePermission
{
    public Guid IdRolePermission { get; set; }

    public Guid PermissionId { get; set; }

    public Guid RoleId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
