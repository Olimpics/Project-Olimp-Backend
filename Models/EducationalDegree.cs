﻿using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalDegree
{
    public int IdEducationalDegree { get; set; }

    public string NameEducationalDegreec { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
