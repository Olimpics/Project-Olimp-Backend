using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentRoleId { get; set; }

    public long? PermissionsMask { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
