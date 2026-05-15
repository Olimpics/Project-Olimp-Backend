using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class TypeOfDiscipline
{
    public string? TypeName { get; set; }

    public Guid IdTypeOfDiscipline { get; set; }

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
