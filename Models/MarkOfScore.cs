using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MarkOfScore
{
    public int? MinGrade { get; set; }

    public int? MaxGrade { get; set; }

    public string? NameOfGrade { get; set; }

    public Guid IdMark { get; set; }
}
