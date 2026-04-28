using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RolesInSg
{
    public int IdRoleSg { get; set; }

    public string? NameRole { get; set; }

    public string? Points { get; set; }

    public virtual ICollection<MembersOfSg> MembersOfSgs { get; set; } = new List<MembersOfSg>();
}
