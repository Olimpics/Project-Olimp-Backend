using OlimpBack.Models;

namespace OlimpBack.DTO
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public virtual Student? Student { get; set; }
    }

    public class LoginResponseDto
    {
        public int? IdStudents { get; set; }
        public int RoleId { get; set; }
        public string? NameStudent { get; set; }
        public string? NameFaculty { get; set; }
        public string? Speciality { get; set; }
        public int? Course { get; set; }
    }
} 