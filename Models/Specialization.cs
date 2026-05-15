using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Specialization
{
    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Avail { get; set; }

    public Guid IdSpecialization { get; set; }

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();
}
