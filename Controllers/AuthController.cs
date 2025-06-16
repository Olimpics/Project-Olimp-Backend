using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Security.Claims;
using AutoMapper;
using BCrypt.Net;
using System.Text.Json;

namespace OlimpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AppDbContext context,
        JwtService jwtService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<AuthController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("authorization")]
    public async Task<ActionResult<LoginResponseWithTokenDto>> Authorization(LoginDto model)
    {
        _logger.LogInformation($"Login attempt for email: {model.Email}");
        
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            _logger.LogWarning($"Login failed: User not found for email {model.Email}");
            return NotFound("This user doesn't exist");
        }

        bool isPasswordValid;
        try
        {
            isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            isPasswordValid = model.Password == user.Password;
        }

        if (!isPasswordValid)
        {
            _logger.LogWarning($"Login failed: Invalid password for user {model.Email}");
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

        LoginResponseWithTokenDto response;

        if (user.Role.NameRole == "Administrator")
        {
            var admin = await _context.AdminsPersonals
                .Include(a => a.Faculty)
                .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

            if (admin == null)
            {
                _logger.LogWarning($"Admin profile not found for user {model.Email}");
                return NotFound("Admin profile not found");
            }

            response = new LoginResponseWithTokenDto()
            {
                Id = admin.IdAdmins,
                UserId = admin.UserId,
                RoleId = user.RoleId,
                Name = admin.NameAdmin,
                NameFaculty = admin.Faculty?.NameFaculty,
                Permissions = permissions
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
                _logger.LogWarning($"Student profile not found for user {model.Email}");
                return NotFound("This student doesn't exist");
            }

            response = _mapper.Map<LoginResponseWithTokenDto>(student);
            response.Permissions = permissions;
        }

        // Generate token
        var token = _jwtService.GenerateToken(
            user.IdUsers.ToString(),
            user.Email,
            user.Role.NameRole
        );

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

        // Set token cookie
        Response.Cookies.Append("AuthToken", token, cookieOptions);

        // Add token to response
        response.Token = token;

        _logger.LogInformation($"Login successful for user {model.Email}. Cookies set.");
        return Ok(response);
    }

    [Authorize]
    [HttpGet("AuthChecker")]
    public async Task<ActionResult<LoginResponseDto>> GetCurrentUser()
    {
        _logger.LogInformation("GetCurrentUser endpoint called");
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("No user ID found in token");
            return Unauthorized("No user ID found in token");
        }

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.IdUsers.ToString() == userId);

        if (user == null)
        {
            _logger.LogWarning($"User not found for ID: {userId}");
            return NotFound("User not found");
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
                _logger.LogWarning($"Admin profile not found for user {user.Email}");
                return NotFound("Admin profile not found");
            }

            response = new LoginResponseDto
            {
                Id = admin.IdAdmins,
                UserId = admin.UserId,
                RoleId = user.RoleId,
                Name = admin.NameAdmin,
                NameFaculty = admin.Faculty?.NameFaculty,
                Permissions = permissions
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
                _logger.LogWarning($"Student profile not found for user {user.Email}");
                return NotFound("This student doesn't exist");
            }

            response = _mapper.Map<LoginResponseDto>(student);
            response.Permissions = permissions;
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

        _logger.LogInformation($"Returning user data for {user.Email}");
        return Ok(response);
    }
} 