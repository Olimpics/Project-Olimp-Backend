using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MembersOfSg
{
    public int IdMember { get; set; }

    public int? SubDivisionId { get; set; }

    public int? RoleInSgid { get; set; }

    public virtual RolesInSg? RoleInSg { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual SubDivisionsSg? SubDivision { get; set; }
}
