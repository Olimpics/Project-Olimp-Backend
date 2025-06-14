using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Achievement
{
    public int IdAchievement { get; set; }

    public string Name { get; set; } = null!;

    public byte[]? Photo { get; set; }
}
