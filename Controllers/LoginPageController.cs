using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

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

        [HttpGet]
        public async Task<ActionResult<LoginResponseDto>> Login([FromQuery] string Email, [FromQuery] string Password)
        {
            _logger.LogInformation($"Login attempt for email: {Email}");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                _logger.LogWarning($"Login failed: User not found for email {Email}");
                return NotFound("This user doesn't exist");
            }

            if (user.Password != Password)
            {
                _logger.LogWarning($"Login failed: Invalid password for user {Email}");
                return BadRequest("Incorrect password");
            }

            // Get user's permissions
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

            // Set secure HTTP-only cookies
            var expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "60");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Path = "/"
            };

            // Set user info cookie
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
    }
} 