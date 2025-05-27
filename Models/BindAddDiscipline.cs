using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindAddDiscipline
{
    public int IdBindAddDisciplines { get; set; }

    public int StudentId { get; set; }

    public int AddDisciplinesId { get; set; }

    public int Semestr { get; set; }

    public int Loans { get; set; }

    public bool InProcess { get; set; }

    public virtual AddDiscipline AddDisciplines { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
