namespace OlimpBack.DTO;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public int IdRole { get; set; }
    public List<PermissionDto> Permissions { get; set; }
} 