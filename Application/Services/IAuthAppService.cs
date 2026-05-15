using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IAuthAppService
{
    Task<Dictionary<string, List<string>>> GetPermissionsByRoleAsync(Guid roleId);

    
    Task<(UserLoginResponseDto? response,
          List<PermissionDto>? permissions,
          string? roleName,
          int? statusCode,
          object? errorPayload)> AuthorizeWithDatabaseAsync(LoginDto model);

    Task<(object? response,
          List<PermissionDto>? permissions,
          int? statusCode,
          string? errorPayload)> GetCurrentUserAsync(Guid userId);

    Task<(bool success, int statusCode, string message)> ChangePasswordAsync(ChangePasswordDto dto);
}
