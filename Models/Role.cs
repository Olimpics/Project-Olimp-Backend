using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public string? NameRole { get; set; }

    public string? Description { get; set; }

    public int? IsSystem { get; set; }
}
