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

    public virtual Student Student { get; set; } = null!;
}
