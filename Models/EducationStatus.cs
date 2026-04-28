using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationStatus
{
    public int IdEducationStatus { get; set; }

    public string? NameEducationStatus { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
