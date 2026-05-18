using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class TypeOfControl
{
    public string Type { get; set; } = null!;

    public Guid IdTypeOfControl { get; set; }

    public virtual ICollection<MainDiscipline> MainDisciplines { get; set; } = new List<MainDiscipline>();

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
