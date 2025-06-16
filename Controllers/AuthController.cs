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
    public async Task<IActionResult> Authorization(LoginDto model)
    {
        _logger.LogInformation($"Login attempt for email: {model.Email}");
        
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            _logger.LogWarning($"Login failed: User not found for email {model.Email}");
            return Unauthorized("Invalid email or password");
        }

        bool isPasswordValid;
        try
        {
            // Try BCrypt verification first
            isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // If BCrypt verification fails, check if it's a plain text password
            isPasswordValid = model.Password == user.Password;
            
            // If it's a plain text password, hash it and update the user's password
            if (isPasswordValid)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                await _context.SaveChangesAsync();
            }
        }

        if (!isPasswordValid)
        {
            _logger.LogWarning($"Login failed: Invalid password for user {model.Email}");
            return Unauthorized("Invalid email or password");
        }

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Get user permissions
        var permissions = await _context.BindRolePermissions
            .Include(b => b.Permission)
            .Where(b => b.RoleId == user.RoleId)
            .Select(b => new PermissionDto
            {
                IdPermissions = b.Permission.IdPermissions,
                NamePermission = b.Permission.TableName,
            })
            .ToListAsync();

        var token = _jwtService.GenerateToken(
            user.IdUsers.ToString(),
            user.Email,
            user.Role.NameRole
        );

        _logger.LogInformation($"Login successful for user {user.Email}. Token generated.");

        // Set secure HTTP-only cookies
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(3),
            Path = "/"
        };

        // Set user info cookie
        var userInfo = new
        {
            UserId = user.IdUsers.ToString(),
            Email = user.Email,
            Role = user.Role.NameRole,
            IdRole = user.RoleId
        };
        Response.Cookies.Append("UserInfo", JsonSerializer.Serialize(userInfo), cookieOptions);

        // Set permissions cookie
        Response.Cookies.Append("UserPermissions", JsonSerializer.Serialize(permissions), cookieOptions);

        // Set token cookie
        Response.Cookies.Append("AuthToken", token, cookieOptions);

        var response = new AuthResponseDto
        {
            Token = token,
            UserId = user.IdUsers.ToString(),
            Email = user.Email,
            Role = user.Role.NameRole,
            IdRole = user.RoleId,
            Permissions = permissions
        };

        return Ok(response);
    }

    [Authorize]
    [HttpGet("AuthChecker")]
    public async Task<IActionResult> GetCurrentUser()
    {
        _logger.LogInformation("GetCurrentUser endpoint called");
        
        var authHeader = Request.Headers["Authorization"].ToString();
        _logger.LogInformation($"Authorization header: {authHeader}");

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            _logger.LogWarning("No Bearer token found in Authorization header");
            return Unauthorized("No Bearer token found in Authorization header");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        _logger.LogInformation($"Extracted token: {token}");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation($"User ID from token: {userId}");

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

        // Get user permissions
        var permissions = await _context.BindRolePermissions
            .Include(b => b.Permission)
            .Where(b => b.RoleId == user.RoleId)
            .Select(b => new PermissionDto
            {
                IdPermissions = b.Permission.IdPermissions,
                NamePermission = b.Permission.TableName,
            })
            .ToListAsync();

        var roleName = user.RoleId > 1 ? "Administrator" : user.Role.NameRole;

        var response = new
        {
            user.IdUsers,
            user.Email,
            Role = roleName,
            IdRole = user.RoleId,
            Permissions = permissions
        };

        _logger.LogInformation($"Returning user data for {user.Email}");
        return Ok(response);
    }
} 