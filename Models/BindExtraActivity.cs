using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindExtraActivity
{
    public int IdBindExtraActivity { get; set; }

    public int StudentId { get; set; }

    public int RefulationId { get; set; }

    public int Points { get; set; }

    public virtual RegulationOnAddPoint Refulation { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
