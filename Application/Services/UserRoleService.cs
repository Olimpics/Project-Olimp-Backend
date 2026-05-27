using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IUserRoleService
{
    Task<IReadOnlyList<UserRoleAssignmentDto>> GetByUserIdAsync(Guid userId);
    Task<UserRoleAssignmentDto?> GetByIdAsync(Guid idUserRole);
    Task<(UserRoleAssignmentDto? dto, int? statusCode, string? errorMessage)> CreateAsync(
        Guid granterUserId, CreateUserRoleAssignmentDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(
        Guid granterUserId, Guid idUserRole, UpdateUserRoleAssignmentDto dto);
}

public class UserRoleService : IUserRoleService
{
    private readonly AppDbContext _context;
    private readonly IRoleKindService _roleKind;

    public UserRoleService(AppDbContext context, IRoleKindService roleKind)
    {
        _context = context;
        _roleKind = roleKind;
    }

    public async Task<IReadOnlyList<UserRoleAssignmentDto>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => new UserRoleAssignmentDto
            {
                IdUserRole = ur.IdUserRole,
                UserId = ur.UserId,
                UserEmail = ur.User.Email,
                RoleId = ur.RoleId,
                RoleName = ur.Role.Name,
                FacultyId = ur.FacultyId,
                DepartmentId = ur.DepartmentId
            })
            .ToListAsync();
    }

    public async Task<UserRoleAssignmentDto?> GetByIdAsync(Guid idUserRole)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.IdUserRole == idUserRole)
            .Select(ur => new UserRoleAssignmentDto
            {
                IdUserRole = ur.IdUserRole,
                UserId = ur.UserId,
                UserEmail = ur.User.Email,
                RoleId = ur.RoleId,
                RoleName = ur.Role.Name,
                FacultyId = ur.FacultyId,
                DepartmentId = ur.DepartmentId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(UserRoleAssignmentDto? dto, int? statusCode, string? errorMessage)> CreateAsync(
        Guid granterUserId, CreateUserRoleAssignmentDto dto)
    {
        var validation = await ValidateReferencesAsync(dto.UserId, dto.RoleId, dto.FacultyId, dto.DepartmentId);
        if (validation != null)
            return (null, validation.Value.statusCode, validation.Value.errorMessage);

        if (!await CanAssignRoleAsync(granterUserId, dto.RoleId))
        {
            return (null, StatusCodes.Status403Forbidden,
                "You cannot assign this role. Target role must be lower than your hierarchy level.");
        }

        if (await AssignmentExistsAsync(dto.UserId, dto.RoleId, dto.FacultyId, dto.DepartmentId))
        {
            return (null, StatusCodes.Status400BadRequest,
                "This user already has the same role for the selected faculty and department.");
        }

        var entity = new UserRole
        {
            IdUserRole = Guid.NewGuid(),
            UserId = dto.UserId,
            RoleId = dto.RoleId,
            FacultyId = dto.FacultyId,
            DepartmentId = dto.DepartmentId
        };

        _context.UserRoles.Add(entity);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entity.IdUserRole), null, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(
        Guid granterUserId, Guid idUserRole, UpdateUserRoleAssignmentDto dto)
    {
        var entity = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.IdUserRole == idUserRole);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "User role assignment not found");

        var validation = await ValidateReferencesAsync(entity.UserId, dto.RoleId, dto.FacultyId, dto.DepartmentId);
        if (validation != null)
            return (false, validation.Value.statusCode, validation.Value.errorMessage);

        if (!await CanAssignRoleAsync(granterUserId, dto.RoleId))
        {
            return (false, StatusCodes.Status403Forbidden,
                "You cannot assign this role. Target role must be lower than your hierarchy level.");
        }

        if (await AssignmentExistsAsync(entity.UserId, dto.RoleId, dto.FacultyId, dto.DepartmentId, idUserRole))
        {
            return (false, StatusCodes.Status400BadRequest,
                "This user already has the same role for the selected faculty and department.");
        }

        entity.RoleId = dto.RoleId;
        entity.FacultyId = dto.FacultyId;
        entity.DepartmentId = dto.DepartmentId;

        await _context.SaveChangesAsync();
        return (true, StatusCodes.Status204NoContent, null);
    }

    private async Task<(int statusCode, string errorMessage)?> ValidateReferencesAsync(
        Guid userId, Guid roleId, Guid facultyId, Guid departmentId)
    {
        if (!await _context.Users.AsNoTracking().AnyAsync(u => u.IdUser == userId))
            return (StatusCodes.Status400BadRequest, "User not found");

        if (!await _context.Roles.AsNoTracking().AnyAsync(r => r.IdRole == roleId))
            return (StatusCodes.Status400BadRequest, "Role not found");

        if (!await _context.Faculties.AsNoTracking().AnyAsync(f => f.IdFaculty == facultyId))
            return (StatusCodes.Status400BadRequest, "Faculty not found");

        var department = await _context.Departments
            .AsNoTracking()
            .Where(d => d.IdDepartment == departmentId)
            .Select(d => new { d.FacultyId })
            .FirstOrDefaultAsync();

        if (department == null)
            return (StatusCodes.Status400BadRequest, "Department not found");

        if (department.FacultyId != facultyId)
            return (StatusCodes.Status400BadRequest, "Department does not belong to the selected faculty");

        return null;
    }

    private async Task<bool> AssignmentExistsAsync(
        Guid userId,
        Guid roleId,
        Guid facultyId,
        Guid departmentId,
        Guid? excludeIdUserRole = null)
    {
        var query = _context.UserRoles.AsNoTracking()
            .Where(ur =>
                ur.UserId == userId &&
                ur.RoleId == roleId &&
                ur.FacultyId == facultyId &&
                ur.DepartmentId == departmentId);

        if (excludeIdUserRole.HasValue)
            query = query.Where(ur => ur.IdUserRole != excludeIdUserRole.Value);

        return await query.AnyAsync();
    }

    private async Task<bool> CanAssignRoleAsync(Guid granterUserId, Guid targetRoleId)
    {
        var granterRoles = await _roleKind.GetUserRoleSnapshotsAsync(granterUserId);
        var targetRole = await _roleKind.GetRoleSnapshotAsync(targetRoleId);

        if (targetRole == null)
            return false;

        return RoleHierarchyCatalog.CanAssignUserRole(granterRoles, targetRole.Value);
    }
}
