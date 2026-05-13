using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class UserRole
{
    public int RoleId { get; set; }

    public int IdUserRole { get; set; }

    public Guid? UserId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User? User { get; set; }
}
