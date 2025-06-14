using OlimpBack.Models;

namespace OlimpBack.DTO
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
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

} 