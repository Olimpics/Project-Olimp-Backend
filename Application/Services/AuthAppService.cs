using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;
using OlimpBack.Utils;

namespace OlimpBack.Application.Services;

public class AuthAppService : IAuthAppService
{
    private readonly IAuthRepository _repository;
    private readonly IMapper _mapper;

    public AuthAppService(IAuthRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Dictionary<string, List<string>>> GetPermissionsByRoleAsync(int roleId)
    {
        var permissions = await _repository.GetRolePermissionsAsync(roleId);

        return permissions
            .GroupBy(p => p.TypePermission)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.TableName).ToList()
            );
    }

    public async Task<(UserLoginResponseDto? response, List<PermissionDto>? permissions, string? roleName, int? statusCode, object? errorPayload)> AuthorizeWithDatabaseAsync(LoginDto model)
    {
        var user = await _repository.GetUserByEmailTrackedAsync(model.Email);

        if (user == null)
            return (null, null, null, StatusCodes.Status404NotFound, "This user doesn't exist");

        var roles = GetOrderedRoles(user);
        var primaryRole = roles.FirstOrDefault();
        if (primaryRole == null)
            return (null, null, null, StatusCodes.Status404NotFound, "User has no role assigned.");

        if (user.PasswordHash == null || user.PasswordHash.Length == 0 ||
            user.PasswordSalt == null || user.PasswordSalt.Length == 0)
        {
            return (null, null, null, StatusCodes.Status400BadRequest, "User has no password set. Please reset password.");
        }

        if (!PasswordHelper.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt))
            return (null, null, null, StatusCodes.Status400BadRequest, "Incorrect password");

        if (IsFirstLogin(user))
        {
            return (null, null, null, StatusCodes.Status403Forbidden,
                new { Message = "Password change required", RequirePasswordChange = true });
        }

        var permissionsDb = await GetUserPermissionsAsync(roles);
        var permissionsMask = await _repository.GetUserPermissionsMaskAsync(user.IdUser);

        UserLoginResponseDto dbResponse;
        if (IsAdminRole(primaryRole))
        {
            var admin = await _repository.GetAdminProfileAsync(user.IdUser);
            if (admin == null)
                return (null, null, null, StatusCodes.Status404NotFound, "Admin profile not found");

            dbResponse = _mapper.Map<LoginResponseAdminDto>(admin);
        }
        else
        {
            var student = await _repository.GetStudentProfileAsync(user.IdUser);
            if (student == null)
                return (null, null, null, StatusCodes.Status404NotFound, "Student not found");

            dbResponse = _mapper.Map<LoginResponseStudentDto>(student);
        }

        user.LastLoginAt = ToPostgresTimestamp(DateTime.UtcNow);
        await _repository.SaveChangesAsync();

        dbResponse.UserId = user.IdUser;
        dbResponse.RoleId = primaryRole.IdRole;
        dbResponse.PermissionsMask = permissionsMask;

        return (dbResponse, permissionsDb, primaryRole.Name, null, null);
    }

    public async Task<(object? response, List<PermissionDto>? permissions, int? statusCode, string? errorPayload)> GetCurrentUserAsync(int userId)
    {
        var user = await _repository.GetUserByIdTrackedAsync(userId);

        if (user == null)
            return (null, null, StatusCodes.Status404NotFound, "User not found");

        var roles = GetOrderedRoles(user);
        var primaryRole = roles.FirstOrDefault();
        if (primaryRole == null)
            return (null, null, StatusCodes.Status404NotFound, "User has no role assigned.");

        var permissions = await GetUserPermissionsAsync(roles);
        var permissionsMask = await _repository.GetUserPermissionsMaskAsync(user.IdUser);

        object? response;
        if (IsAdminRole(primaryRole))
        {
            var admin = await _repository.GetAdminProfileAsync(user.IdUser);
            if (admin == null)
                return (null, null, StatusCodes.Status404NotFound, "Admin profile not found");

            var adminResponse = _mapper.Map<LoginResponseAdminDto>(admin);
            adminResponse.UserId = user.IdUser;
            adminResponse.RoleId = primaryRole.IdRole;
            adminResponse.PermissionsMask = permissionsMask;
            response = adminResponse;
        }
        else
        {
            var student = await _repository.GetStudentProfileAsync(user.IdUser);
            if (student == null)
                return (null, null, StatusCodes.Status404NotFound, "This student doesn't exist");

            var studentResponse = _mapper.Map<LoginResponseStudentDto>(student);
            studentResponse.UserId = user.IdUser;
            studentResponse.RoleId = primaryRole.IdRole;
            studentResponse.PermissionsMask = permissionsMask;
            response = studentResponse;
        }

        return (response, permissions, null, null);
    }

    public async Task<(bool success, int statusCode, string message)> ChangePasswordAsync(ChangePasswordDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.NewPassword))
            return (false, StatusCodes.Status400BadRequest, "Email and new password are required.");

        var user = await _repository.GetUserByEmailTrackedAsync(dto.Email);
        if (user == null)
            return (false, StatusCodes.Status404NotFound, "User not found.");

        if (!IsFirstLogin(user))
        {
            if (string.IsNullOrEmpty(dto.OldPassword))
                return (false, StatusCodes.Status400BadRequest, "Old password is required.");

            if (user.PasswordHash == null || user.PasswordHash.Length == 0 ||
                user.PasswordSalt == null || user.PasswordSalt.Length == 0)
            {
                return (false, StatusCodes.Status400BadRequest, "User has no password set.");
            }

            if (!PasswordHelper.VerifyPassword(dto.OldPassword, user.PasswordHash, user.PasswordSalt))
                return (false, StatusCodes.Status400BadRequest, "Old password incorrect.");
        }

        var (isValid, error) = PasswordHelper.ValidatePasswordPolicy(dto.NewPassword);
        if (!isValid)
            return (false, StatusCodes.Status400BadRequest, error);

        PasswordHelper.CreatePasswordHash(dto.NewPassword, out var newHash, out var newSalt);

        user.PasswordHash = newHash;
        user.PasswordSalt = newSalt;
        user.IsFirstLogin = new BitArray(1, false);
        user.PasswordChangedAt = ToPostgresTimestamp(DateTime.UtcNow);

        await _repository.SaveChangesAsync();

        return (true, StatusCodes.Status200OK, "Password changed successfully.");
    }

    private async Task<List<PermissionDto>> GetUserPermissionsAsync(IReadOnlyCollection<Role> roles)
    {
        var byId = new Dictionary<int, PermissionDto>();

        foreach (var role in roles)
        {
            var rolePermissions = await _repository.GetRolePermissionsAsync(role.IdRole);
            foreach (var permission in rolePermissions)
                byId[permission.IdPermissions] = permission;
        }

        return byId.Values.OrderBy(p => p.BitIndex).ToList();
    }

    private static List<Role> GetOrderedRoles(User user)
    {
        return user.UserRoles
            .Select(ur => ur.Role)
            .Where(role => role != null)
            .OrderByDescending(IsAdminRole)
            .ThenBy(role => role.IdRole)
            .ToList();
    }

    private static bool IsAdminRole(Role role)
    {
        return role.Name.Contains("admin", StringComparison.OrdinalIgnoreCase) ||
               role.Name.Contains("administrator", StringComparison.OrdinalIgnoreCase) ||
               role.IdRole > 1;
    }

    private static bool IsFirstLogin(User user)
    {
        return user.IsFirstLogin.Cast<bool>().FirstOrDefault();
    }

    private static DateTime ToPostgresTimestamp(DateTime value)
    {
        return DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
    }
}
