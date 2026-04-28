//using Microsoft.EntityFrameworkCore;
//using OlimpBack.Application.DTO;
//using OlimpBack.Application.Permissions;
//using OlimpBack.Infrastructure.Database;
//using OlimpBack.Models;
//using OlimpBack.Data;

//namespace OlimpBack.Infrastructure.Database.Repositories;

//public interface IAuthRepository
//{
//    Task<User?> GetUserByEmailTrackedAsync(string email);
//    Task<User?> GetUserByIdTrackedAsync(int userId);
//    Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId);
//    Task<long> GetRolePermissionsMaskAsync(int roleId);
//    Task<long> GetUserPermissionsMaskAsync(int userId);
//    Task<Student?> GetStudentProfileAsync(int userId);
//    Task<AdminsPersonal?> GetAdminProfileAsync(int userId);
//    Task SaveChangesAsync();
//}

//public class AuthRepository : IAuthRepository
//{
//    private readonly AppDbContext _context;
//    private readonly IRoleMaskService _roleMaskService;

//    public AuthRepository(AppDbContext context, IRoleMaskService roleMaskService)
//    {
//        _context = context;
//        _roleMaskService = roleMaskService;
//    }

//    public async Task<User?> GetUserByEmailTrackedAsync(string email)
//    {
//        return await _context.Users
//            .Include(u => u.PrimaryRole)
//            .FirstOrDefaultAsync(u => u.Email == email);
//    }

//    public async Task<User?> GetUserByIdTrackedAsync(int userId)
//    {
//        return await _context.Users
//            .Include(u => u.PrimaryRole)
//            .FirstOrDefaultAsync(u => u.IdUser == userId);
//    }

//    public async Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId)
//    {
//        return await _context.Set<Role1>()
//            .AsNoTracking()
//            .Where(r => r.Id == roleId)
//            .SelectMany(r => r.Permissions)
//            .Select(p => new PermissionDto
//            {
//                IdPermissions = p.Id,
//                TypePermission = "S",
//                TableName = p.Code,
//                BitIndex = p.BitIndex
//            })
//            .ToListAsync();
//    }

//    public async Task<long> GetRolePermissionsMaskAsync(int roleId)
//    {
//        return await _roleMaskService.GetRoleMaskAsync(roleId);
//    }

//    public async Task<long> GetUserPermissionsMaskAsync(int userId)
//    {
//        return await _roleMaskService.GetUserPermissionsMaskAsync(userId);
//    }

//    public async Task<Student?> GetStudentProfileAsync(int userId)
//    {
//        return await _context.Students
//            .AsNoTracking()
//            .Include(x => x.Faculty)
//            .Include(x => x.EducationalProgram)
//            .Include(x => x.EducationalDegree)
//            .Include(x => x.User)!.ThenInclude(u => u!.PrimaryRole)
//            .FirstOrDefaultAsync(x => x.UserId == userId);
//    }

//    public async Task<AdminsPersonal?> GetAdminProfileAsync(int userId)
//    {
//        return await _context.AdminsPersonals
//            .AsNoTracking()
//            .Include(x => x.Faculty)
//            .Include(x => x.User)!.ThenInclude(u => u!.PrimaryRole)
//            .FirstOrDefaultAsync(x => x.UserId == userId);
//    }

//    public async Task SaveChangesAsync()
//    {
//        await _context.SaveChangesAsync();
//    }
//}
