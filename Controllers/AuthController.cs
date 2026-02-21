using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Security.Claims;
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

        // ============================================================
        // 🔹 STUB LOGIN (без БД)
        // ============================================================
        if (_configuration.GetValue<bool>("UseStubLogin"))
        {
            _logger.LogWarning("STUB AUTHORIZATION MODE ENABLED");

            LoginResponseWithTokenDto response;
            List<PermissionDto> permissions;

            // ====== ADMIN ======
            if (model.Email == "admin@admin")
            {
                response = new LoginResponseWithTokenDto
                {
                    Id = 4,
                    UserId = 6,
                    RoleId = 2,
                    Name = "Test Admin",
                    NameFaculty = "Faculty of Computer Science",
                    DegreeLevel = null
                };

                permissions = new List<PermissionDto>
            {
                    

    // ===== SELECT (S) =====
    new() { TypePermission = "S", TableName = "Achievement" },
    new() { TypePermission = "S", TableName = "AddDetails" },
    new() { TypePermission = "S", TableName = "AddDisciplines" },
    new() { TypePermission = "S", TableName = "AdminLogs" },
    new() { TypePermission = "S", TableName = "AdminsPersonal" },
    new() { TypePermission = "S", TableName = "BindAddDisciplines" },
    new() { TypePermission = "S", TableName = "BindEvents" },
    new() { TypePermission = "S", TableName = "BindExtraActivity" },
    new() { TypePermission = "S", TableName = "BindLoansMain" },
    new() { TypePermission = "S", TableName = "BindMainDisciplines" },
    new() { TypePermission = "S", TableName = "BindRolePermission" },
    new() { TypePermission = "S", TableName = "Department" },
    new() { TypePermission = "S", TableName = "EducationalDegree" },
    new() { TypePermission = "S", TableName = "EducationalProgram" },
    new() { TypePermission = "S", TableName = "EducationStatus" },
    new() { TypePermission = "S", TableName = "Events" },
    new() { TypePermission = "S", TableName = "Faculties" },
    new() { TypePermission = "S", TableName = "Group" },
    new() { TypePermission = "S", TableName = "MainGrade" },
    new() { TypePermission = "S", TableName = "Members" },
    new() { TypePermission = "S", TableName = "Notifications" },
    new() { TypePermission = "S", TableName = "NotificationTemplates" },
    new() { TypePermission = "S", TableName = "Permissions" },
    new() { TypePermission = "S", TableName = "RegulationOnAddPoints" },
    new() { TypePermission = "S", TableName = "Role" },
    new() { TypePermission = "S", TableName = "RolesInSG" },
    new() { TypePermission = "S", TableName = "Student" },
    new() { TypePermission = "S", TableName = "StudyForm" },
    new() { TypePermission = "S", TableName = "SubDivisionsSG" },
    new() { TypePermission = "S", TableName = "Users" },

    // ===== INSERT (I) =====
    new() { TypePermission = "I", TableName = "Achievement" },
    new() { TypePermission = "I", TableName = "AddDetails" },
    new() { TypePermission = "I", TableName = "AddDisciplines" },
    new() { TypePermission = "I", TableName = "AdminsPersonal" },
    new() { TypePermission = "I", TableName = "BindAddDisciplines" },
    new() { TypePermission = "I", TableName = "BindEvents" },
    new() { TypePermission = "I", TableName = "BindExtraActivity" },
    new() { TypePermission = "I", TableName = "BindLoansMain" },
    new() { TypePermission = "I", TableName = "BindMainDisciplines" },
    new() { TypePermission = "I", TableName = "BindRolePermission" },
    new() { TypePermission = "I", TableName = "Department" },
    new() { TypePermission = "I", TableName = "EducationalDegree" },
    new() { TypePermission = "I", TableName = "EducationalProgram" },
    new() { TypePermission = "I", TableName = "EducationStatus" },
    new() { TypePermission = "I", TableName = "Events" },
    new() { TypePermission = "I", TableName = "Faculties" },
    new() { TypePermission = "I", TableName = "Group" },
    new() { TypePermission = "I", TableName = "MainGrade" },
    new() { TypePermission = "I", TableName = "Members" },
    new() { TypePermission = "I", TableName = "Notifications" },
    new() { TypePermission = "I", TableName = "NotificationTemplates" },
    new() { TypePermission = "I", TableName = "Permissions" },
    new() { TypePermission = "I", TableName = "RegulationOnAddPoints" },
    new() { TypePermission = "I", TableName = "Role" },
    new() { TypePermission = "I", TableName = "RolesInSG" },
    new() { TypePermission = "I", TableName = "Student" },
    new() { TypePermission = "I", TableName = "StudyForm" },
    new() { TypePermission = "I", TableName = "SubDivisionsSG" },
    new() { TypePermission = "I", TableName = "Users" },

    // ===== UPDATE (A) =====
    new() { TypePermission = "U", TableName = "Achievement" },
    new() { TypePermission = "U", TableName = "AddDetails" },
    new() { TypePermission = "U", TableName = "AddDisciplines" },
    new() { TypePermission = "U", TableName = "AdminsPersonal" },
    new() { TypePermission = "U", TableName = "BindAddDisciplines" },
    new() { TypePermission = "U", TableName = "BindEvents" },
    new() { TypePermission = "U", TableName = "BindExtraActivity" },
    new() { TypePermission = "U", TableName = "BindLoansMain" },
    new() { TypePermission = "U", TableName = "BindMainDisciplines" },
    new() { TypePermission = "U", TableName = "BindRolePermission" },
    new() { TypePermission = "U", TableName = "Department" },
    new() { TypePermission = "U", TableName = "EducationalDegree" },
    new() { TypePermission = "U", TableName = "EducationalProgram" },
    new() { TypePermission = "U", TableName = "EducationStatus" },
    new() { TypePermission = "U", TableName = "Events" },
    new() { TypePermission = "U", TableName = "Faculties" },
    new() { TypePermission = "U", TableName = "Group" },
    new() { TypePermission = "U", TableName = "MainGrade" },
    new() { TypePermission = "U", TableName = "Members" },
    new() { TypePermission = "U", TableName = "Notifications" },
    new() { TypePermission = "U", TableName = "NotificationTemplates" },
    new() { TypePermission = "U", TableName = "Permissions" },
    new() { TypePermission = "U", TableName = "RegulationOnAddPoints" },
    new() { TypePermission = "U", TableName = "Role" },
    new() { TypePermission = "U", TableName = "RolesInSG" },
    new() { TypePermission = "U", TableName = "Student" },
    new() { TypePermission = "U", TableName = "StudyForm" },
    new() { TypePermission = "U", TableName = "SubDivisionsSG" },
    new() { TypePermission = "U", TableName = "Users" },

    // ===== DELETE (D) =====
    new() { TypePermission = "D", TableName = "Achievement" },
    new() { TypePermission = "D", TableName = "AddDetails" },
    new() { TypePermission = "D", TableName = "AddDisciplines" },
    new() { TypePermission = "D", TableName = "AdminsPersonal" },
    new() { TypePermission = "D", TableName = "BindAddDisciplines" },
    new() { TypePermission = "D", TableName = "BindEvents" },
    new() { TypePermission = "D", TableName = "BindExtraActivity" },
    new() { TypePermission = "D", TableName = "BindLoansMain" },
    new() { TypePermission = "D", TableName = "BindMainDisciplines" },
    new() { TypePermission = "D", TableName = "BindRolePermission" },
    new() { TypePermission = "D", TableName = "Department" },
    new() { TypePermission = "D", TableName = "EducationalDegree" },
    new() { TypePermission = "D", TableName = "EducationalProgram" },
    new() { TypePermission = "D", TableName = "EducationStatus" },
    new() { TypePermission = "D", TableName = "Events" },
    new() { TypePermission = "D", TableName = "Faculties" },
    new() { TypePermission = "D", TableName = "Group" },
    new() { TypePermission = "D", TableName = "MainGrade" },
    new() { TypePermission = "D", TableName = "Members" },
    new() { TypePermission = "D", TableName = "Notifications" },
    new() { TypePermission = "D", TableName = "NotificationTemplates" },
    new() { TypePermission = "D", TableName = "Permissions" },
    new() { TypePermission = "D", TableName = "RegulationOnAddPoints" },
    new() { TypePermission = "D", TableName = "Role" },
    new() { TypePermission = "D", TableName = "RolesInSG" },
    new() { TypePermission = "D", TableName = "Student" },
    new() { TypePermission = "D", TableName = "StudyForm" },
    new() { TypePermission = "D", TableName = "SubDivisionsSG" },
    new() { TypePermission = "D", TableName = "Users" }


                };
            }
            // ====== STUDENT ======
            else
            {
                response = new LoginResponseWithTokenDto
                {
                    Id = 44072,
                    UserId = 3483,
                    RoleId = 1,
                    Name = "Test Student",
                    NameFaculty = "Faculty of Computer Science",
                    DegreeLevel = "Bachelor"
                };

                permissions = new List<PermissionDto>
            {
                // ===== SELECT (S) =====
    new() { TypePermission = "S", TableName = "Achievement" },
    new() { TypePermission = "S", TableName = "AddDetails" },
    new() { TypePermission = "S", TableName = "AddDisciplines" },
    new() { TypePermission = "S", TableName = "BindAddDisciplines" },
    new() { TypePermission = "S", TableName = "Department" },
    new() { TypePermission = "S", TableName = "EducationalDegree" },
    new() { TypePermission = "S", TableName = "EducationalProgram" },
    new() { TypePermission = "S", TableName = "EducationStatus" },
    new() { TypePermission = "S", TableName = "Events" },
    new() { TypePermission = "S", TableName = "Faculties" },
    new() { TypePermission = "S", TableName = "Group" },
    new() { TypePermission = "S", TableName = "MainGrade" },
    new() { TypePermission = "S", TableName = "Members" },
    new() { TypePermission = "S", TableName = "Notifications" },
    new() { TypePermission = "S", TableName = "Permissions" },
    new() { TypePermission = "S", TableName = "RegulationOnAddPoints" },
    new() { TypePermission = "S", TableName = "RolesInSG" },
    new() { TypePermission = "S", TableName = "Student" },
    new() { TypePermission = "S", TableName = "StudyForm" },
    new() { TypePermission = "S", TableName = "SubDivisionsSG" },
    new() { TypePermission = "S", TableName = "Users" },
            };
            }

            // ====== TOKEN ======
            var token = _jwtService.GenerateToken(
                response.UserId.ToString(),
                model.Email,
                response.RoleId == 2 ? "Administrator" : "Student"
            );

            response.Token = token;

            var expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "60");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Path = "/"
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            Response.Cookies.Append("UserInfo", JsonSerializer.Serialize(new
            {
                response.Id,
                response.UserId,
                response.RoleId,
                response.Name,
                response.NameFaculty,
                response.DegreeLevel
            }), cookieOptions);

            Response.Cookies.Append("UserPermissions",
                JsonSerializer.Serialize(permissions),
                cookieOptions);

            return Ok(response);
        }

        // ============================================================
        // 🔹 REAL AUTHORIZATION (через БД) — ТВОЙ КОД
        // ============================================================

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            _logger.LogWarning($"Login failed: User not found for email {model.Email}");
            return NotFound("This user doesn't exist");
        }

        bool isPasswordValid;

        if (user.PasswordHash?.Length > 0 && user.PasswordSalt?.Length > 0)
        {
            isPasswordValid = PasswordHelper.VerifyPassword(
                model.Password,
                user.PasswordHash,
                user.PasswordSalt
            );
        }
        else
        {
            return BadRequest("User has no password set. Please reset password.");
        }

        if (!isPasswordValid)
            return BadRequest("Incorrect password");

        if ((bool)user.IsFirstLogin)
            return StatusCode(StatusCodes.Status403Forbidden,
                new { Message = "Password change required", RequirePasswordChange = true });

        var permissionsDb = await _context.BindRolePermissions
            .Include(b => b.Permission)
            .Where(b => b.RoleId == user.RoleId)
            .Select(b => new PermissionDto
            {
                TypePermission = b.Permission.TypePermission,
                TableName = b.Permission.TableName
            })
            .ToListAsync();

        LoginResponseWithTokenDto dbResponse;

        if (user.Role.IdRole > 1)
        {
            var admin = await _context.AdminsPersonals
                .Include(a => a.Faculty)
                .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

            if (admin == null)
                return NotFound("Admin profile not found");

            dbResponse = _mapper.Map<LoginResponseWithTokenDto>(admin);
        }
        else
        {
            var student = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.EducationalProgram)
                .Include(s => s.EducationalDegree)
                .FirstOrDefaultAsync(s => s.UserId == user.IdUsers);

            if (student == null)
                return NotFound("Student not found");

            dbResponse = _mapper.Map<LoginResponseWithTokenDto>(student);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var jwt = _jwtService.GenerateToken(
            user.IdUsers.ToString(),
            user.Email,
            user.Role.NameRole
        );

        dbResponse.Token = jwt;

        var cookieOpts = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "60")),
            Path = "/"
        };

        Response.Cookies.Append("AuthToken", jwt, cookieOpts);
        Response.Cookies.Append("UserInfo",
            JsonSerializer.Serialize(dbResponse),
            cookieOpts);
        Response.Cookies.Append("UserPermissions",
            JsonSerializer.Serialize(permissionsDb),
            cookieOpts);

        return Ok(dbResponse);
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


    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.NewPassword))
            return BadRequest("Email and new password are required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return NotFound("User not found.");

        if ((bool)!user.IsFirstLogin)
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