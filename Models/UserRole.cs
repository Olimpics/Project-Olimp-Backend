using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class UserRole
{
    public Guid UserId { get; set; }

    public Guid IdUserRole { get; set; }

    public Guid RoleId { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? GroupId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual StudentGroup? Group { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
