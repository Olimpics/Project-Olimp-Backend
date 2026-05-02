using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindLoansMain
{
    public int IdBindLoan { get; set; }

    public int? SelectiveDisciplinesId { get; set; }

    public int? EducationalProgramId { get; set; }

    public virtual SelectiveDiscipline? SelectiveDisciplines { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }
}
