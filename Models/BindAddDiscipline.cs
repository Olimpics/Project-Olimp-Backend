using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Bindadddiscipline
{
    public int? IdBindAddDisciplines { get; set; }

    public int? StudentId { get; set; }

    public int? AddDisciplinesId { get; set; }

    public int? Semestr { get; set; }

    public int? Loans { get; set; }

    public int? InProcess { get; set; }

    public string? Grade { get; set; }

    public string? CreatedAt { get; set; }
}
