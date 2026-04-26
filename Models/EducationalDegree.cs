using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Educationaldegree
{
    public int? IdEducationalDegree { get; set; }

    public string? NameEducationalDegreec { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
