using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindLoansMain
{
    public Guid? EducationalProgramId { get; set; }

    public Guid? SelectiveDisciplinesId { get; set; }

    public Guid IdBindLoanMain { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual SelectiveDiscipline? SelectiveDisciplines { get; set; }
}
