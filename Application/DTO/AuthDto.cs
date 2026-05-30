using System;

namespace OlimpBack.Application.DTO;

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class ProfileLookupDto
{
    public required string Email { get; set; }
}

public class UserProfileDto
{
    public string Email { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public string? Group { get; set; }
}

public class AuthorizationByProfileDto
{
    public Guid UserId { get; set; }
    public required string Password { get; set; }
    public bool? IsAdmin { get; set; }
}
public class ChangePasswordDto
{
    public required string Email { get; set; }
    public string? OldPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class PermissionDto
{
    public Guid IdPermissions { get; set; }
    public string TypePermission { get; set; } = null!;
    public string TableName { get; set; } = null!;
    public int BitIndex { get; set; }
}
public class UserLoginResponseDto
{
    public Guid? Id { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? ThirdName { get; set; }

    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string? Email { get; set; }
    public long PermissionsMask { get; set; }
    public string Token { get; set; } = null!;

}


public class LoginResponseStudentDto : UserLoginResponseDto
{
    public Guid? FacultyId { get; set; }
    public string? NameFaculty { get; set; }
    public string? Speciality { get; set; }
    public int? Course { get; set; }
    public string? DegreeLevel { get; set; }

}

public class LoginResponseAdminDto : UserLoginResponseDto
{
    public Guid? FacultyId { get; set; }
    public string? NameFaculty { get; set; }
}
