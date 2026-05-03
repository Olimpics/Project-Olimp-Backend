using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindExtraActivity
{
    public int IdBindExtraActivity { get; set; }

    public int? StudentId { get; set; }

    public int? RegulationId { get; set; }

    public int? Points { get; set; }

    public string? NameExtraActivity { get; set; }

    public virtual RegulationOnAddPoint? Regulation { get; set; }

    public virtual Student? Student { get; set; }
}
