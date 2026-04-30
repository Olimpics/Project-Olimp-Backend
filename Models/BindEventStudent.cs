using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindEventStudent
{
    public int IdBindEventStudent { get; set; }

    public int? StudentId { get; set; }

    public int? EventId { get; set; }

    public int? Point { get; set; }

    public int? RoleId { get; set; }

    public string? OtherOpion { get; set; }

    public virtual Event? Event { get; set; }

    public virtual RoleInEvent? Role { get; set; }

    public virtual Student? Student { get; set; }
}
