using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public int Id { get => IdRole; set => IdRole = value; }

    public string Name { get; set; } = null!;

    public int? ParentRoleId { get; set; }

    public long? PermissionsMask { get; set; }

    public virtual Role? ParentRole { get; set; }

    public virtual ICollection<Role> ChildRoles { get; set; } = new List<Role>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
