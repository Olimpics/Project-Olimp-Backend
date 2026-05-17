using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class UserRole
{
    public Guid UserId { get; set; }

    public Guid IdUserRole { get; set; }

    public Guid RoleId { get; set; }

    public Guid FacultyId { get; set; }

    public Guid DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
