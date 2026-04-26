using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Bindloansmain
{
    public int? IdBindLoan { get; set; }

    public int? AddDisciplinesId { get; set; }

    public int? EducationalProgramId { get; set; }

    public virtual Adddiscipline? AddDisciplines { get; set; }

    public virtual Educationalprogram? EducationalProgram { get; set; }
}
