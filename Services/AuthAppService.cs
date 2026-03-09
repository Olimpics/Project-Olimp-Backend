using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Utils;

namespace OlimpBack.Services;

public class AuthAppService : IAuthAppService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AuthAppService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Dictionary<string, List<string>>> GetPermissionsByRoleAsync(int roleId)
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

        return permissions
            .GroupBy(p => p.TypePermission)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.TableName).ToList()
            );
    }

    public async Task<(UserLoginResponseDto? response,
        List<PermissionDto>? permissions,
        string? roleName,
        int? statusCode,
        object? errorPayload)> AuthorizeWithDatabaseAsync(LoginDto model)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            return (null, null, null,
                StatusCodes.Status404NotFound,
                "This user doesn't exist");
        }

        if (user.PasswordHash is null || user.PasswordSalt is null ||
            user.PasswordHash.Length == 0 || user.PasswordSalt.Length == 0)
        {
            return (null, null, null,
                StatusCodes.Status400BadRequest,
                "User has no password set. Please reset password.");
        }

        var isPasswordValid = PasswordHelper.VerifyPassword(
            model.Password,
            user.PasswordHash,
            user.PasswordSalt
        );

        if (!isPasswordValid)
        {
            return (null, null, null,
                StatusCodes.Status400BadRequest,
                "Incorrect password");
        }

        if (user.IsFirstLogin == true)
        {
            return (null, null, null,
                StatusCodes.Status403Forbidden,
                new { Message = "Password change required", RequirePasswordChange = true });
        }

        var permissionsDb = await _context.BindRolePermissions
            .Include(b => b.Permission)
            .Where(b => b.RoleId == user.RoleId)
            .Select(b => new PermissionDto
            {
                TypePermission = b.Permission.TypePermission,
                TableName = b.Permission.TableName
            })
            .ToListAsync();

        UserLoginResponseDto dbResponse;

        if (user.Role.IdRole > 1)
        {
            var admin = await _context.AdminsPersonals
                .Include(a => a.Faculty)
                .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

            if (admin == null)
            {
                return (null, null, null,
                    StatusCodes.Status404NotFound,
                    "Admin profile not found");
            }

            dbResponse = _mapper.Map<LoginResponseAdminDto>(admin);
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
                return (null, null, null,
                    StatusCodes.Status404NotFound,
                    "Student not found");
            }

            dbResponse = _mapper.Map<LoginResponseStudentDto>(student);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (dbResponse, permissionsDb, user.Role.NameRole, null, null);
    }

    public async Task<(object? response,
        List<PermissionDto>? permissions,
        int? statusCode,
        string? errorPayload)> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.IdUsers == userId);

        if (user == null)
        {
            return (null, null,
                StatusCodes.Status404NotFound,
                "User not found");
        }

        var permissions = await _context.BindRolePermissions
            .Include(b => b.Permission)
            .Where(b => b.RoleId == user.RoleId)
            .Select(b => new PermissionDto
            {
                TypePermission = b.Permission.TypePermission,
                TableName = b.Permission.TableName
            })
            .ToListAsync();

        object? response = null;

        if (user.Role.IdRole > 1)
        {
            var admin = await _context.AdminsPersonals
                .Include(a => a.Faculty)
                .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

            if (admin == null)
            {
                return (null, null,
                    StatusCodes.Status404NotFound,
                    "Admin profile not found");
            }

            response = _mapper.Map<LoginResponseStudentDto>(admin);
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
                return (null, null,
                    StatusCodes.Status404NotFound,
                    "This student doesn't exist");
            }

            response = _mapper.Map<LoginResponseStudentDto>(student);
        }

        return (response, permissions, null, null);
    }

    public async Task<(bool success, int statusCode, string message)> ChangePasswordAsync(ChangePasswordDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.NewPassword))
        {
            return (false, StatusCodes.Status400BadRequest,
                "Email and new password are required.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            return (false, StatusCodes.Status404NotFound,
                "User not found.");
        }

        if (user.IsFirstLogin == false)
        {
            if (string.IsNullOrEmpty(dto.OldPassword))
            {
                return (false, StatusCodes.Status400BadRequest,
                    "Old password is required.");
            }

            if (user.PasswordHash is null || user.PasswordSalt is null)
            {
                return (false, StatusCodes.Status400BadRequest,
                    "User has no password set.");
            }

            if (!PasswordHelper.VerifyPassword(dto.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return (false, StatusCodes.Status400BadRequest,
                    "Old password incorrect.");
            }
        }

        var (isValid, error) = PasswordHelper.ValidatePasswordPolicy(dto.NewPassword);
        if (!isValid)
        {
            return (false, StatusCodes.Status400BadRequest, error);
        }

        PasswordHelper.CreatePasswordHash(dto.NewPassword, out var newHash, out var newSalt);
        user.PasswordHash = newHash;
        user.PasswordSalt = newSalt;
        user.IsFirstLogin = false;
        user.PasswordChangedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (true, StatusCodes.Status200OK, "Password changed successfully.");
    }
}

