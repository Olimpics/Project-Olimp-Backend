using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Specialization
{
    public int IdSpecialization { get; set; }

    public float? Code { get; set; }

    public string? Name { get; set; }

    public int? IdSpeciality { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<BindEpspecialization> BindEpspecializations { get; set; } = new List<BindEpspecialization>();
}
