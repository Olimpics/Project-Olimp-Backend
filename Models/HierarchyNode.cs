using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class HierarchyNode
{
    public Guid IdNode { get; set; }

    public Guid TreeId { get; set; }

    public Guid? ParentId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int ManagementWeight { get; set; }

    public virtual ICollection<HierarchyNode> InverseParent { get; set; } = new List<HierarchyNode>();

    public virtual HierarchyNode? Parent { get; set; }

    public virtual HierarchyTree Tree { get; set; } = null!;

    public virtual ICollection<UserHierarchyAssignment> UserHierarchyAssignments { get; set; } = new List<UserHierarchyAssignment>();
}
