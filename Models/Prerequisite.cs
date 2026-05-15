using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Prerequisite
{
    public Guid? EducationalProgramId { get; set; }

    public Guid? SelectiveDisciplineId { get; set; }

    public Guid IdPrerequisites { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual SelectiveDiscipline? SelectiveDiscipline { get; set; }
}
