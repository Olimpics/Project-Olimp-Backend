namespace OlimpBack.Application.DTO;

public class UserHierarchyAssignmentDto
{
    public Guid IdAssignment { get; set; }

    public Guid UserId { get; set; }

    public string UserEmail { get; set; } = null!;

    public Guid HierarchyNodeId { get; set; }

    public string HierarchyNodeName { get; set; } = null!;

    public int ManagementWeight { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? GroupId { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateUserHierarchyAssignmentDto
{
    public Guid UserId { get; set; }

    public Guid HierarchyNodeId { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? GroupId { get; set; }
}

public class UpdateUserHierarchyAssignmentDto
{
    public Guid HierarchyNodeId { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? GroupId { get; set; }
}
