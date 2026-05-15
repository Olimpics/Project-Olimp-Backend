using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindExtraActivity
{
    public int? Points { get; set; }

    public string? NameExtraActivity { get; set; }

    public Guid StudentId { get; set; }

    public Guid? RegulationId { get; set; }

    public Guid IdBindExtraActivity { get; set; }

    public virtual RegulationOnAddPoint? Regulation { get; set; }

    public virtual Student Student { get; set; } = null!;
}
