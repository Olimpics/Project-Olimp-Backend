namespace OlimpBack.DTO
{
    public class UserRoleDto
    {
        public int IdUsers { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
    }
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }

    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
