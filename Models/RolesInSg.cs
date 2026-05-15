using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RolesInSg
{
    public string NameRole { get; set; } = null!;

    public int PointsFac { get; set; }

    public int PointsUni { get; set; }

    public Guid IdRoleSg { get; set; }

    public virtual ICollection<BindSubdivisionRoleSg> BindSubdivisionRoleSgs { get; set; } = new List<BindSubdivisionRoleSg>();
}
