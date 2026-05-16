using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindSimilaEducationalProgramInGroup
{
    public Guid IdBind { get; set; }

    public Guid GroupId { get; set; }

    public Guid EducationalProgramId { get; set; }

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;

    public virtual GroupSimilarEducationalProgram Group { get; set; } = null!;
}
