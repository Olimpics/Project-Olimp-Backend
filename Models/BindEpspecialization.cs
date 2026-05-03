using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindEpspecialization
{
    public int? SpecializationId { get; set; }

    public int? EducationalProgramId { get; set; }

    public int IdBind { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual Specialization? Specialization { get; set; }
}
