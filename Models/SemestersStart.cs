using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class SemestersStart
{
    public DateOnly StartDate { get; set; }

    public Guid IdSemestrStart { get; set; }
}
