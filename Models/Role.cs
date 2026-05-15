using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Role
{
    public string Name { get; set; } = null!;

    public long? PermissionsMask { get; set; }

    public Guid? ParentRoleId { get; set; }

    public Guid IdRole { get; set; }

    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
