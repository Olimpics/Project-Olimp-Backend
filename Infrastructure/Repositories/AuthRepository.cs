using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailTrackedAsync(string email);
    Task<User?> GetUserByIdTrackedAsync(int userId);
    Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId);
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

    //  :  AsNoTracking(),     LastLoginAt  Password
    public async Task<User?> GetUserByEmailTrackedAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByIdTrackedAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.IdUsers == userId);
    }

    public async Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId)
    {
        // : Include(Permission)   , EF Core   JOIN
        return await _context.BindRolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => new PermissionDto
            {
                IdPermissions = x.Permission.IdPermissions,
                TypePermission = x.Permission.TypePermission,
                TableName = x.Permission.TableName,
                BitIndex = x.Permission.BitIndex
            })
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
        //     
        return await _context.Students
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.EducationalProgram)
            .Include(x => x.EducationalDegree)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<AdminsPersonal?> GetAdminProfileAsync(int userId)
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
}