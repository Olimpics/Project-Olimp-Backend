namespace OlimpBack.Application.DTO;

public class AssignableFacultyDto
{
    public Guid IdFaculty { get; set; }

    public string NameFaculty { get; set; } = null!;

    public string? Abbreviation { get; set; }
}

public class AssignableDepartmentDto
{
    public Guid IdDepartment { get; set; }

    public Guid FacultyId { get; set; }

    public string NameDepartment { get; set; } = null!;

    public string? Abbreviation { get; set; }
}

public class AssignableGroupDto
{
    public Guid IdGroup { get; set; }

    public string GroupCode { get; set; } = null!;

    public Guid FacultyId { get; set; }

    public Guid DepartmentId { get; set; }
}

public class AssignableRoleDto
{
    public Guid IdRole { get; set; }

    public string NameRole { get; set; } = null!;

    public int EffectiveWeight { get; set; }

    public Guid? CreatedInFacultyId { get; set; }

    public Guid? CreatedInDepartmentId { get; set; }

    public Guid? CreatedInGroupId { get; set; }
}

public class AssignablePermissionDto
{
    public Guid IdPermission { get; set; }

    public string Code { get; set; } = null!;

    public int BitIndex { get; set; }

    public int RequiredWeight { get; set; }
}
