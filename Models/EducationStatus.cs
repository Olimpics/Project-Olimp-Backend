using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationStatus
{
    public string? NameEducationStatus { get; set; }

    public Guid IdEducationStatus { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
