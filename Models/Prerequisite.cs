using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Prerequisite
{
    public int IdPrerequisites { get; set; }

    public int? EducationalProgramId { get; set; }

    public int? SelectiveDisciplensId { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual SelectiveDiscipline? SelectiveDisciplens { get; set; }
}
