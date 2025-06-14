using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SubDivisionsSg
{
    public int IdSubDivision { get; set; }

    public string NameDivision { get; set; } = null!;

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
