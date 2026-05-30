using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class UserHierarchyAssignment
{
    public Guid IdAssignment { get; set; }

    public Guid UserId { get; set; }

    public Guid HierarchyNodeId { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? GroupId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual StudentGroup? Group { get; set; }

    public virtual HierarchyNode HierarchyNode { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
