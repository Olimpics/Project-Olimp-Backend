using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RolesInSg
{
    public int IdRoleSg { get; set; }

    public string? NameRole { get; set; }

    public string? PointsFac { get; set; }

    public string? PointsUni { get; set; }

    public virtual ICollection<BindSubdivisionRoleSg> BindSubdivisionRoleSgs { get; set; } = new List<BindSubdivisionRoleSg>();
}
