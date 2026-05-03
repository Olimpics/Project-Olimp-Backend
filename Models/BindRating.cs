using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindRating
{
    public int IdBindRating { get; set; }

    public int? StudentId { get; set; }

    public int? Year { get; set; }

    public BitArray? Semestr { get; set; }

    public float? FinalScore { get; set; }

    public BitArray? IsRedo { get; set; }

    public virtual Student? Student { get; set; }
}
