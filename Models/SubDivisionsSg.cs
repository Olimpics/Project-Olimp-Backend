using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SubDivisionsSg
{
    public int IdSubDivision { get; set; }

    public string? NameDivision { get; set; }

    public virtual ICollection<MembersOfSg> MembersOfSgs { get; set; } = new List<MembersOfSg>();
}
