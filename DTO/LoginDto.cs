using OlimpBack.Models;

namespace OlimpBack.DTO
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class PermissionDto
    {
        public int IdPermissions { get; set; }
        public string TypePermission { get; set; } = null!;
        public string TableName { get; set; } = null!;
    }

    public class LoginResponseDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int RoleId { get; set; }
        public string? Name{ get; set; }
        public string? NameFaculty { get; set; }
        public string? Speciality { get; set; }
        public int? Course { get; set; }

    }

    public class LoginResponseWithTokenDto : LoginResponseDto
    {
        public string Token { get; set; }
    }
} 
