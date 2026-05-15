using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindEventStudent
{
    public int? Point { get; set; }

    public string? OtherOption { get; set; }

    public Guid StudentId { get; set; }

    public Guid? RoleId { get; set; }

    public Guid IdBindEventStudent { get; set; }

    public Guid? EventId { get; set; }

    public virtual Event? Event { get; set; }

    public virtual RoleInEvent? Role { get; set; }

    public virtual Student Student { get; set; } = null!;
}
