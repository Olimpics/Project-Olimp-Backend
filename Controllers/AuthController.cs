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

    [HttpGet("permissions/{roleId}")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> GetPermissionsByRoleId(int roleId)
    {
        var permissions = await _context.BindRolePermissions
            .Include(b => b.Permission)
            .Where(b => b.RoleId == roleId)
            .Select(b => new PermissionDto
            {
                TypePermission = b.Permission.TypePermission,
                TableName = b.Permission.TableName
            })
            .ToListAsync();

        var groupedPermissions = permissions
            .GroupBy(p => p.TypePermission)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.TableName).ToList()
            );

        return Ok(groupedPermissions);
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

        bool isPasswordValid = false;

        // 1) Если у пользователя есть PBKDF2 хеш/соль -> проверяем через PasswordHelper
        if (user.PasswordHash != null && user.PasswordHash.Length > 0 && user.PasswordSalt != null && user.PasswordSalt.Length > 0)
        {
            try
            {
                isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while verifying PBKDF2 password for user {model.Email}");
                isPasswordValid = false;
            }
        }
        else
        {

            // Нет и хеша, и старого пароля — не можем проверить
            _logger.LogWarning($"Login failed: No password data for user {model.Email}");
            return BadRequest("User has no password set. Please reset password.");
        }

        if (!isPasswordValid)
        {
            _logger.LogWarning($"Login failed: Invalid password for user {model.Email}");
            return BadRequest("Incorrect password");
        }

        // Если пароль верный, но требуется принудительная смена при первом входе — сообщаем клиенту
        if (user.IsFirstLogin)
        {
            _logger.LogInformation($"User {model.Email} must change password on first login.");
            // Возвращаем 403 и флаг, чтобы фронт понял — показать экран смены пароля
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Password change required on first login.", RequirePasswordChange = true });
        }

        // Получаем права пользователя
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

        if (user.Role.IdRole > 1)
        {
            var admin = await _context.AdminsPersonals
                .Include(a => a.Faculty)
                .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

            if (admin == null)
            {
                _logger.LogWarning($"Admin profile not found for user {model.Email}");
                return NotFound("Admin profile not found");
            }

            response = _mapper.Map<LoginResponseWithTokenDto>(admin);
        }
        else
        {
            var student = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.EducationalProgram)
                .Include(s => s.EducationalDegree)
                .FirstOrDefaultAsync(s => s.UserId == user.IdUsers);

            if (student == null)
            {
                _logger.LogWarning($"Student profile not found for user {model.Email}");
                return NotFound("This student doesn't exist");
            }

            response = _mapper.Map<LoginResponseWithTokenDto>(student);
        }

        // Обновляем LastLoginAt
        try
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Failed to update LastLoginAt for user {model.Email}");
            // Не прерываем вход — логируем и продолжаем
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
            response.Id,
            response.UserId,
            response.RoleId,
            response.Name,
            response.NameFaculty,
            response.DegreeLevel
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

        if (user.Role.IdRole > 1)
        {
            var admin = await _context.AdminsPersonals
                .Include(a => a.Faculty)
                .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

            if (admin == null)
            {
                _logger.LogWarning($"Admin profile not found for user {user.Email}");
                return NotFound("Admin profile not found");
            }

            response = _mapper.Map<LoginResponseDto>(admin);
        }
        else
        {
            var student = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.EducationalProgram)
                .Include(s => s.EducationalDegree)
                .FirstOrDefaultAsync(s => s.UserId == user.IdUsers);

            if (student == null)
            {
                _logger.LogWarning($"Student profile not found for user {user.Email}");
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
            response.Id,
            response.UserId,
            response.RoleId,
            response.Name,
            response.NameFaculty,
            response.DegreeLevel
        };
        Response.Cookies.Append("UserInfo", JsonSerializer.Serialize(userInfo), cookieOptions);

        // Set permissions cookie
        Response.Cookies.Append("UserPermissions", JsonSerializer.Serialize(permissions), cookieOptions);

        _logger.LogInformation($"Returning user data for {user.Email}");
        return Ok(response);
    }
}