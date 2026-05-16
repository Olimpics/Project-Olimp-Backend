using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindRating
{
    public float FinalScore { get; set; }

    public bool IsEven { get; set; }

    public Guid IdBindRating { get; set; }

    public bool IsRedo { get; set; }

    public Guid StudentId { get; set; }

    public virtual Student Student { get; set; } = null!;
}
