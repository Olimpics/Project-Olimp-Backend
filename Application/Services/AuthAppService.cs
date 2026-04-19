using AutoMapper;
using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
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

        // ˜˜˜˜˜˜-˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜ ˜˜˜˜˜
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
        
        if (user.PasswordHash == null || user.PasswordHash.Length == 0 ||
            user.PasswordSalt == null || user.PasswordSalt.Length == 0)
        {
            return (null, null, null, StatusCodes.Status400BadRequest, "User has no password set. Please reset password.");
        }
        var isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt);

        if (!isPasswordValid)
            return (null, null, null, StatusCodes.Status400BadRequest, "Incorrect password");

        if (user.IsFirstLogin == true)
        {
            return (null, null, null, StatusCodes.Status403Forbidden,
                new { Message = "Password change required", RequirePasswordChange = true });
        }

        var permissionsDb = await _repository.GetRolePermissionsAsync(user.RoleId);
        var permissionsMask = await _repository.GetUserPermissionsMaskAsync(user.IdUsers);

        UserLoginResponseDto dbResponse;

        if (user.Role.IdRole > 1)
        {
            var admin = await _repository.GetAdminProfileAsync(user.IdUsers);
            if (admin == null)
                return (null, null, null, StatusCodes.Status404NotFound, "Admin profile not found");

            dbResponse = _mapper.Map<LoginResponseAdminDto>(admin);
        }
        else
        {
            var student = await _repository.GetStudentProfileAsync(user.IdUsers);
            if (student == null)
                return (null, null, null, StatusCodes.Status404NotFound, "Student not found");

            dbResponse = _mapper.Map<LoginResponseStudentDto>(student);
        }

        // ˜˜˜˜˜˜˜ Tracking ˜ ˜˜˜˜˜˜˜˜˜, ˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜ ˜ ˜˜˜˜˜˜˜˜
        user.LastLoginAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();

        dbResponse.PermissionsMask = permissionsMask;

        return (dbResponse, permissionsDb, user.Role.NameRole, null, null);
    }

    public async Task<(object? response, List<PermissionDto>? permissions, int? statusCode, string? errorPayload)> GetCurrentUserAsync(int userId)
    {
        var user = await _repository.GetUserByIdTrackedAsync(userId);

        if (user == null)
            return (null, null, StatusCodes.Status404NotFound, "User not found");

        var permissions = await _repository.GetRolePermissionsAsync(user.RoleId);
        var permissionsMask = await _repository.GetUserPermissionsMaskAsync(user.IdUsers);

        object? response = null;

        if (user.Role.IdRole > 1)
        {
            var admin = await _repository.GetAdminProfileAsync(user.IdUsers);
            if (admin == null)
                return (null, null, StatusCodes.Status404NotFound, "Admin profile not found");

            // ˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜: ˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜ ˜ AdminDto, ˜ ˜˜ ˜ StudentDto!
            var adminResponse = _mapper.Map<LoginResponseAdminDto>(admin);
            adminResponse.PermissionsMask = permissionsMask;
            response = adminResponse;
        }
        else
        {
            var student = await _repository.GetStudentProfileAsync(user.IdUsers);
            if (student == null)
                return (null, null, StatusCodes.Status404NotFound, "This student doesn't exist");

            var studentResponse = _mapper.Map<LoginResponseStudentDto>(student);
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

        if (user.IsFirstLogin == false)
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
        user.IsFirstLogin = false;
        user.PasswordChangedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return (true, StatusCodes.Status200OK, "Password changed successfully.");
    }
}