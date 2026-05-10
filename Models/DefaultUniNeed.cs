using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DefaultUniNeed
{
    public int IdUniNeeds { get; set; }

    public int? EducationalDegreeId { get; set; }

    public int? Count { get; set; }

    public BitArray? IsAccelerated { get; set; }

    public BitArray? SemestrIsEven { get; set; }

    public int? Choice { get; set; }

    public virtual EducationalDegree? EducationalDegree { get; set; }
}
