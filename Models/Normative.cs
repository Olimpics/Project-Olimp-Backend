using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Normative
{
    public int? Count { get; set; }

    public Guid? DegreeLevelId { get; set; }

    public bool IsFaculty { get; set; }

    public Guid IdNormative { get; set; }

    public virtual EducationalDegree? DegreeLevel { get; set; }
}
