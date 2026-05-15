using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Branch
{
    public int Code { get; set; }

    public string Name { get; set; } = null!;

    public Guid IdBranch { get; set; }

    public bool Avail { get; set; }

    public virtual ICollection<Speciality> Specialities { get; set; } = new List<Speciality>();
}
