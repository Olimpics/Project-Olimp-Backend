namespace OlimpBack.Application.DTO;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class ChangePasswordDto
{
    public string Email { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}

public class PermissionDto
{
    public int IdPermissions { get; set; }
    public string TypePermission { get; set; } = null!;
    public string TableName { get; set; } = null!;
}
public class UserLoginResponseDto
{

    
    public int? Id { get; set; }
    public string? Name { get; set; }

    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Token { get; set; }

}


public class LoginResponseStudentDto : UserLoginResponseDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int FacultyId { get; set; }
    public string? NameFaculty { get; set; }
    public string? Speciality { get; set; }
    public int? Course { get; set; }
    public string? DegreeLevel { get; set; }

}

public class LoginResponseAdminDto : UserLoginResponseDto
{
    public int FacultyId { get; set; }
    public string? NameFaculty { get; set; }
}
