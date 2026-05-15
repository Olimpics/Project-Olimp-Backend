using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class TypeOfControl
{
    public string? Type { get; set; }

    public Guid IdTypeOfControl { get; set; }

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
