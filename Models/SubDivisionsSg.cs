using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SubDivisionsSg
{
    public int IdSubDivision { get; set; }

    public string? NameDivision { get; set; }

    public virtual ICollection<BindSubdivisionRoleSg> BindSubdivisionRoleSgs { get; set; } = new List<BindSubdivisionRoleSg>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
