using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SubDivisionsSg
{
    public string NameDivision { get; set; } = null!;

    public bool Avail { get; set; }

    public Guid IdSubDivisions { get; set; }

    public virtual ICollection<BindSubdivisionRoleSg> BindSubdivisionRoleSgs { get; set; } = new List<BindSubdivisionRoleSg>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
