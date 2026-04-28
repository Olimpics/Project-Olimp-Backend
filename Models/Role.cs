using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentRoleId { get; set; }

    public long? PermissionsMask { get; set; }
}
