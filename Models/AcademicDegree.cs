using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AcademicDegree
{
    public Guid IdAcademicDegree { get; set; }

    public string AcademicDegreeName { get; set; } = null!;

    public string AcademicDegreeShortedName { get; set; } = null!;

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();
}
