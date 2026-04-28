using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Branch
{
    public int IdBranch { get; set; }

    public int? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Speciality> Specialities { get; set; } = new List<Speciality>();
}
