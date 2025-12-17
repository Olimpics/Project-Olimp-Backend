using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Text.Json;

namespace OlimpBack.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginPageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginPageController> _logger;

        public LoginPageController(
            AppDbContext context,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<LoginPageController> logger)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        // DTOs
        public class RegisterDto
        {
            public string Email { get; set; }
            public string Password { get; set; } 
            public int RoleId { get; set; }
        }

        public class ChangePasswordDto
        {
            public string Email { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists) return BadRequest("User with this email already exists.");

            string password = dto.Password;
            bool generatePassword = false;
            if (string.IsNullOrEmpty(password))
            {
                password = PasswordHelper.GeneratePassword(12);
                generatePassword = true;
            }

            var (isValid, error) = PasswordHelper.ValidatePasswordPolicy(password);
            if (!isValid) return BadRequest(error);

            PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = dto.RoleId,
                IsFirstLogin = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"New user registered: {dto.Email}");

            if (generatePassword)
            {
                return Ok(new { Message = "User created. Password was generated. Please store it securely.", GeneratedPassword = password });
            }

            return Ok(new { Message = "User created." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var Email = request.Email;
            var Password = request.Password;

            _logger.LogInformation($"Login attempt for email: {Email}");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                _logger.LogWarning($"Login failed: User not found for email {Email}");
                return NotFound("This user doesn't exist");
            }

            if (!PasswordHelper.VerifyPassword(Password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning($"Login failed: Invalid password for user {Email}");
                return BadRequest("Incorrect password");
            }

            if (user.IsFirstLogin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Password change required on first login.", RequireChange = true });
            }

            var permissions = await _context.BindRolePermissions
                           .Include(b => b.Permission)
                           .Where(b => b.RoleId == user.RoleId)
                           .Select(b => new PermissionDto
                           {
                               TypePermission = b.Permission.TypePermission,
                               TableName = b.Permission.TableName
                           })
                           .ToListAsync();

            LoginResponseDto response;

            if (user.Role.NameRole == "Administrator")
            {
                var admin = await _context.AdminsPersonals
                    .Include(a => a.Faculty)
                    .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

                if (admin == null)
                {
                    _logger.LogWarning($"Admin profile not found for user {Email}");
                    return NotFound("Admin profile not found");
                }

                response = new LoginResponseDto
                {
                    Id = admin.IdAdmins,
                    UserId = admin.UserId,
                    RoleId = user.RoleId,
                    Name = admin.NameAdmin,
                    NameFaculty = admin.Faculty?.NameFaculty
                };
            }
            else
            {
                var student = await _context.Students
                    .Include(s => s.Faculty)
                    .Include(s => s.EducationalProgram)
                    .FirstOrDefaultAsync(s => s.UserId == user.IdUsers);

                if (student == null)
                {
                    _logger.LogWarning($"Student profile not found for user {Email}");
                    return NotFound("This student doesn't exist");
                }

                response = _mapper.Map<LoginResponseDto>(student);
            }

            var expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "60");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Path = "/"
            };

            var userInfo = new
            {
                Id = response.Id,
                UserId = response.UserId,
                RoleId = response.RoleId,
                Name = response.Name,
                NameFaculty = response.NameFaculty
            };
            Response.Cookies.Append("UserInfo", JsonSerializer.Serialize(userInfo), cookieOptions);

            // Set permissions cookie
            Response.Cookies.Append("UserPermissions", JsonSerializer.Serialize(permissions), cookieOptions);

            _logger.LogInformation($"Login successful for user {Email}. Cookies set.");
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest("Email and new password are required.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound("User not found.");

            if (!user.IsFirstLogin)
            {
                if (string.IsNullOrEmpty(dto.OldPassword))
                    return BadRequest("Old password is required.");

                if (!PasswordHelper.VerifyPassword(dto.OldPassword, user.PasswordHash, user.PasswordSalt))
                    return BadRequest("Old password incorrect.");
            }
            var (isValid, error) = PasswordHelper.ValidatePasswordPolicy(dto.NewPassword);
            if (!isValid) return BadRequest(error);

            PasswordHelper.CreatePasswordHash(dto.NewPassword, out var newHash, out var newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;
            user.IsFirstLogin = false;
            user.PasswordChangedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password changed for user {dto.Email}");

            return Ok(new { Message = "Password changed successfully." });
        }
    }


}