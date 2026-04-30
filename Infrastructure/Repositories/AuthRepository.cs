using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailTrackedAsync(string email);
    Task<User?> GetUserByIdTrackedAsync(int userId);
    Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId);
    Task<List<Role>> GetUserRolesAsync(int userId);
    Task<long> GetRolePermissionsMaskAsync(int roleId);
    Task<long> GetUserPermissionsMaskAsync(int userId);
    Task<Student?> GetStudentProfileAsync(int userId);
    Task<AdminsPersonal?> GetAdminProfileAsync(int userId);
    Task SaveChangesAsync();
}

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly IRoleMaskService _roleMaskService;

    public AuthRepository(AppDbContext context, IRoleMaskService roleMaskService)
    {
        _context = context;
        _roleMaskService = roleMaskService;
    }

    public async Task<User?> GetUserByEmailTrackedAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByIdTrackedAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.IdUser == userId);
    }

    public async Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId)
    {
        return await _context.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => new PermissionDto
            {
                IdPermissions = rp.Permission.Id,
                TypePermission = GetPermissionAction(rp.Permission.Code),
                TableName = GetPermissionResource(rp.Permission.Code),
                BitIndex = rp.Permission.BitIndex
            })
            .OrderBy(p => p.BitIndex)
            .ToListAsync();
    }

    public async Task<List<Role>> GetUserRolesAsync(int userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .OrderBy(r => r.IdRole)
            .ToListAsync();
    }

    public async Task<long> GetRolePermissionsMaskAsync(int roleId)
    {
        return await _roleMaskService.GetRoleMaskAsync(roleId);
    }

    public async Task<long> GetUserPermissionsMaskAsync(int userId)
    {
        return await _roleMaskService.GetUserPermissionsMaskAsync(userId);
    }

    public async Task<Student?> GetStudentProfileAsync(int userId)
    {
        return await _context.Students
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.EducationalProgram)
            .Include(x => x.EducationalDegree)
            .Include(x => x.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<AdminsPersonal?> GetAdminProfileAsync(int userId)
    {
        return await _context.AdminsPersonals
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.User)
                .ThenInclude(u => u!.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private static string GetPermissionResource(string code)
    {
        var parts = code.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : code;
    }

    private static string GetPermissionAction(string code)
    {
        var parts = code.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : string.Empty;
    }
}
