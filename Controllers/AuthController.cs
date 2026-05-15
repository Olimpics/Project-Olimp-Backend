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
    public async Task<ActionResult<Dictionary<string, List<string>>>> GetPermissionsByRoleId(Guid roleId)
    {
        var groupedPermissions = await _authAppService.GetPermissionsByRoleAsync(roleId);
        return Ok(groupedPermissions);
    }

    [HttpPost("authorization")]
    public async Task<ActionResult<UserLoginResponseDto>> Authorization(LoginDto model)
    {
        _logger.LogInformation($"Login attempt for email: {model.Email}");

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