using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int BitIndex { get; set; }

    public virtual ICollection<Role1> Roles { get; set; } = new List<Role1>();
}
