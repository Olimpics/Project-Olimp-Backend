using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class RolesInSg
{
    public string NameRole { get; set; } = null!;

    public Guid IdRoleSg { get; set; }

    public virtual ICollection<BindSubdivisionRoleSg> BindSubdivisionRoleSgs { get; set; } = new List<BindSubdivisionRoleSg>();
}
