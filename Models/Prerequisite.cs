using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Prerequisite
{
    public int Idprerequisites { get; set; }

    public int? Educationalprogramid { get; set; }

    public int? Adddisciplensid { get; set; }

    public virtual SelectiveDiscipline? Adddisciplens { get; set; }

    public virtual EducationalProgram? Educationalprogram { get; set; }
}
