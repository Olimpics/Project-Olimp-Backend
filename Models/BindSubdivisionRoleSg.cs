using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindSubdivisionRoleSg
{
    public int? Points { get; set; }

    public Guid SubDivisionId { get; set; }

    public Guid? RoleInSgid { get; set; }

    public Guid IdBindSubdivisionRoleSg { get; set; }

    public virtual ICollection<MembersOfSg> MembersOfSgs { get; set; } = new List<MembersOfSg>();

    public virtual RolesInSg? RoleInSg { get; set; }

    public virtual SubDivisionsSg SubDivision { get; set; } = null!;
}
