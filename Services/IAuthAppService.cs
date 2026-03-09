using OlimpBack.DTO;

namespace OlimpBack.Services;

public interface IAuthAppService
{
    Task<Dictionary<string, List<string>>> GetPermissionsByRoleAsync(int roleId);

    Task<(LoginResponseWithTokenDto? response,
          List<PermissionDto>? permissions,
          string? roleName,
          int? statusCode,
          object? errorPayload)> AuthorizeWithDatabaseAsync(LoginDto model);

    Task<(object? response,
          List<PermissionDto>? permissions,
          int? statusCode,
          string? errorPayload)> GetCurrentUserAsync(int userId);

    Task<(bool success, int statusCode, string message)> ChangePasswordAsync(ChangePasswordDto dto);
}

