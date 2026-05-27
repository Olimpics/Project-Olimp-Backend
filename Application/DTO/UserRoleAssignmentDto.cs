namespace OlimpBack.Application.DTO;

public class UserRoleAssignmentDto
{
    public Guid IdUserRole { get; set; }

    public Guid UserId { get; set; }

    public string UserEmail { get; set; } = null!;

    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public Guid FacultyId { get; set; }

    public Guid DepartmentId { get; set; }
}

public class CreateUserRoleAssignmentDto
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public Guid FacultyId { get; set; }

    public Guid DepartmentId { get; set; }
}

public class UpdateUserRoleAssignmentDto
{
    public Guid RoleId { get; set; }

    public Guid FacultyId { get; set; }

    public Guid DepartmentId { get; set; }
}
