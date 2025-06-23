using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Normative
{
    public int IdNormative { get; set; }

    public string DegreeLevel { get; set; } = null!;

    public int Count { get; set; }
}
