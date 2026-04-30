using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;
using OlimpBack.Utils;
using System.Security.Claims;
using System.Text.Json;

namespace OlimpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IAuthAppService _authAppService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        JwtService jwtService,
        IConfiguration configuration,
        IAuthAppService authAppService,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _configuration = configuration;
        _authAppService = authAppService;
        _logger = logger;
    }

    [HttpGet("permissions/{roleId}")]
    [RequirePermission(RbacPermissions.PermissionsRead)]
    public async Task<ActionResult<Dictionary<string, List<string>>>> GetPermissionsByRoleId(int roleId)
    {
        var groupedPermissions = await _authAppService.GetPermissionsByRoleAsync(roleId);
        return Ok(groupedPermissions);
    }

    [HttpPost("authorization")]
    public async Task<ActionResult<UserLoginResponseDto>> Authorization(LoginDto model)
    {
        _logger.LogInformation($"Login attempt for email: {model.Email}");

        // ============================================================
        // 🔹 STUB LOGIN (без БД)
        // ============================================================
        if (_configuration.GetValue<bool>("UseStubLogin"))
        {
            _logger.LogWarning("STUB AUTHORIZATION MODE ENABLED");

            UserLoginResponseDto response;
            List<PermissionDto> permissions;

            // ====== ADMIN ======
            if (model.Email == "admin@admin")
            {
                response = new LoginResponseAdminDto
                {
                    Id = 4,
                    UserId = 6,
                    RoleId = 2,
                    Name = "Test Admin",
                    NameFaculty = "Faculty of Computer Science",
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
    new() { TypePermission = "S", TableName = "MainDisciplines" },
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
    new() { TypePermission = "I", TableName = "MainDisciplines" },
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
    new() { TypePermission = "U", TableName = "MainDisciplines" },
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
    new() { TypePermission = "D", TableName = "MainDisciplines" },
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
                response = new LoginResponseStudentDto
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

            for (var i = 0; i < permissions.Count; i++)
                permissions[i].BitIndex = i;

            response.PermissionsMask = PermissionMaskHelper.BuildMask(permissions.Select(p => p.BitIndex));

            // ====== TOKEN ======
            var token = _jwtService.GenerateToken(
                response.UserId.ToString() ?? string.Empty,
                model.Email,
                response.RoleId == 2 ? "Administrator" : "Student",
                response.PermissionsMask
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
                response.UserId,
                response.RoleId
            }), cookieOptions);

            Response.Cookies.Append("UserPermissions",
                JsonSerializer.Serialize(permissions),
                cookieOptions);
            Response.Cookies.Append("UserPermissionsMask",
                response.PermissionsMask.ToString(),
                cookieOptions);

            return Ok(response);
        }

        var (dbResponse, permissionsDb, roleName, statusCode, errorPayload) =
            await _authAppService.AuthorizeWithDatabaseAsync(model);

        if (statusCode.HasValue)
        {
            _logger.LogWarning("Authorization failed for {Email} with status {StatusCode}", model.Email, statusCode);
            return StatusCode(statusCode.Value, errorPayload);
        }

        if (dbResponse is null || permissionsDb is null || string.IsNullOrEmpty(roleName))
        {
            _logger.LogError("Authorization service returned an unexpected null result for {Email}", model.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, "Authorization failed.");
        }

        var jwt = _jwtService.GenerateToken(
            dbResponse.UserId.ToString() ?? string.Empty,
            model.Email,
            roleName,
            dbResponse.PermissionsMask
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
        Response.Cookies.Append("UserPermissionsMask",
            dbResponse.PermissionsMask.ToString(),
            cookieOpts);

        return Ok(dbResponse);
    }


    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var (success, statusCode, message) = await _authAppService.ChangePasswordAsync(dto);

        if (!success)
        {
            _logger.LogWarning("Password change failed for {Email}: {Message}", dto.Email, message);
            return StatusCode(statusCode, message);
        }

        _logger.LogInformation("Password changed for user {Email}", dto.Email);

        return Ok(new { Message = message });
    }
}