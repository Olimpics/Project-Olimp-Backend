using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailTrackedAsync(string email);
    Task<User?> GetUserByIdTrackedAsync(Guid userId);
    Task<List<PermissionDto>> GetRolePermissionsAsync(Guid roleId);
    Task<List<Role>> GetUserRolesAsync(Guid userId);
    Task<long> GetRolePermissionsMaskAsync(Guid roleId);
    Task<long> GetUserPermissionsMaskAsync(Guid userId);
    Task<Student?> GetStudentProfileAsync(Guid userId);
    Task<AdminsPersonal?> GetAdminProfileAsync(Guid userId);
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
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByIdTrackedAsync(Guid userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.IdUser == userId);
    }

    public async Task<List<PermissionDto>> GetRolePermissionsAsync(Guid roleId)
    {
        var permissions = await _context.Permissions
            .Where(p => _context.RolePermissions
                .Any(rp => rp.RoleId == roleId && rp.PermissionId == p.IdPermission))
            .OrderBy(p => p.BitIndex)
            .AsNoTracking()
            .ToListAsync();

        return permissions
            .Select(permission => new PermissionDto
            {
                IdPermissions = permission.IdPermission,
                TypePermission = GetPermissionAction(permission.Code),
                TableName = GetPermissionResource(permission.Code),
                BitIndex = permission.BitIndex
            })
            .ToList();
    }

    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.Roles
            .Where(r => _context.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == r.IdRole))
            .OrderBy(r => r.IdRole)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<long> GetRolePermissionsMaskAsync(Guid roleId)
    {
        return await _roleMaskService.GetRoleMaskAsync(roleId);
    }

    public async Task<long> GetUserPermissionsMaskAsync(Guid userId)
    {
        return await _roleMaskService.GetUserPermissionsMaskAsync(userId);
    }

    public async Task<Student?> GetStudentProfileAsync(Guid userId)
    {
        return await _context.Students
            .AsNoTracking()
            .Include(x => x.Group.EducationalProgram.Speciality.Department.Faculty)
            .Include(x => x.Group.EducationalProgram)
            .Include(x => x.Group.EducationalProgram.Degree)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<AdminsPersonal?> GetAdminProfileAsync(Guid userId)
    {
        return await _context.AdminsPersonals
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.User)
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
