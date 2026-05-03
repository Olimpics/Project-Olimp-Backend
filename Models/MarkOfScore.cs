using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MarkOfScore
{
    public int? IdMark { get; set; }

    public int? MinGrade { get; set; }

    public int? MaxGrade { get; set; }

    public string? NameOfGrade { get; set; }
}
