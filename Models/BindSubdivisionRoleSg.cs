using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindSubdivisionRoleSg
{
    public int IdBindSubdivisionRoleSg { get; set; }

    public int? SubDivisionId { get; set; }

    public int? RoleInSgid { get; set; }

    public int? Points { get; set; }

    public virtual ICollection<MembersOfSg> MembersOfSgs { get; set; } = new List<MembersOfSg>();

    public virtual RolesInSg? RoleInSg { get; set; }

    public virtual SubDivisionsSg? SubDivision { get; set; }
}
