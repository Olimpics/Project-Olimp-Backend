using System;

namespace OlimpBack.Application.DTO
{
    public class UserRoleDto
    {
        public Guid IdUsers { get; set; }
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
    public class CreateUserDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid RoleId { get; set; }
    }

    public class UpdateUserDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid RoleId { get; set; }
    }
}
