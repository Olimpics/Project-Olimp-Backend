using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindLoansMain
{
    public int IdBindLoan { get; set; }

    public int AddDisciplinesId { get; set; }

    public int EducationalProgramId { get; set; }

    public virtual AddDiscipline AddDisciplines { get; set; } = null!;

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;
}
