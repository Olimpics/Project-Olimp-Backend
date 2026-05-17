using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Specialization
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool Avail { get; set; }

    public Guid IdSpecialization { get; set; }

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();
}
