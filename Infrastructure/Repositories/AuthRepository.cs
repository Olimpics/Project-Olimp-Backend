using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailTrackedAsync(string email);
    Task<User?> GetUserByIdTrackedAsync(int userId);
    Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId);
    Task<Student?> GetStudentProfileAsync(int userId);
    Task<AdminsPersonal?> GetAdminProfileAsync(int userId);
    Task SaveChangesAsync();
}

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    // ТУТ ВАЖЛИВО: Немає AsNoTracking(), бо ми будемо оновлювати LastLoginAt та Password
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
        // Проекція: Include(Permission) тут не потрібен, EF Core сам зробить JOIN
        return await _context.BindRolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => new PermissionDto
            {
                TypePermission = x.Permission.TypePermission,
                TableName = x.Permission.TableName
            })
            .ToListAsync();
    }

    public async Task<Student?> GetStudentProfileAsync(int userId)
    {
        // Профілі тягнемо суто для читання
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