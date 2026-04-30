using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class TypeOfDiscipline
{
    public int IdTypeOfDiscipline { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
