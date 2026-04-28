using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Typeofcontroll
{
    public int Idtypeofcontroll { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Adddetail> Adddetails { get; set; } = new List<Adddetail>();
}
