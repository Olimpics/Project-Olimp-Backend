using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RolesInSg
{
    public int IdRoleSg { get; set; }

    public string NameRole { get; set; } = null!;

    public int Points { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
