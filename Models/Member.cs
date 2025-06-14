using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Member
{
    public int IdMember { get; set; }

    public int StudentId { get; set; }

    public int SubDivisionId { get; set; }

    public int RoleInSgid { get; set; }

    public virtual RolesInSg RoleInSg { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual SubDivisionsSg SubDivision { get; set; } = null!;
}
