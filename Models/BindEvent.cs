using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindEvent
{
    public int IdBindEvent { get; set; }

    public int StudentId { get; set; }

    public int EventId { get; set; }

    public int? Points { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
