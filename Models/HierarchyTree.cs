using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class HierarchyTree
{
    public Guid IdTree { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<HierarchyNode> HierarchyNodes { get; set; } = new List<HierarchyNode>();
}
