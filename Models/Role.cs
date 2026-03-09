using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public string NameRole { get; set; } = null!;

    public string? Description { get; set; }

    public sbyte? IsSystem { get; set; }

    public virtual ICollection<BindRolePermission> BindRolePermissions { get; set; } = new List<BindRolePermission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
