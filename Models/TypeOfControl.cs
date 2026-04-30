using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class TypeOfControl
{
    public int Idtypeofcontroll { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<SelectiveDetail> SelectiveDetails { get; set; } = new List<SelectiveDetail>();
}
