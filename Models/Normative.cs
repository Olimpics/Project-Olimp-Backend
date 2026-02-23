using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Normative
{
    public int IdNormative { get; set; }

    public int Count { get; set; }

    public sbyte IsFaculty { get; set; }

    public int? DegreeLevelId { get; set; }
}
