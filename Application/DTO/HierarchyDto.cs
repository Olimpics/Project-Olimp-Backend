namespace OlimpBack.Application.DTO;

public class HierarchyTreeDto
{
    public Guid IdTree { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;
}

public class HierarchyNodeDto
{
    public Guid IdNode { get; set; }

    public Guid TreeId { get; set; }

    public Guid? ParentId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int ManagementWeight { get; set; }
}
