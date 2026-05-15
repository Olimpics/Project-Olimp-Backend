using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class DefaultUniNeed
{
    public int? Count { get; set; }

    public int? Choice { get; set; }

    public bool? SemestrIsEven { get; set; }

    public bool? IsAccelerated { get; set; }

    public Guid? EducationalDegreeId { get; set; }

    public Guid IdUniNeeds { get; set; }

    public virtual EducationalDegree? EducationalDegree { get; set; }
}
