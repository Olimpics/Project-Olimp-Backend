using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Data;
using OlimpBack.Utils;

namespace OlimpBack.Application.Services;

public interface IDelegationOptionsService
{
    Task<IReadOnlyList<AssignableFacultyDto>> GetAssignableFacultiesAsync(Guid userId);
    Task<IReadOnlyList<AssignableDepartmentDto>> GetAssignableDepartmentsAsync(Guid userId);
    Task<IReadOnlyList<AssignableGroupDto>> GetAssignableGroupsAsync(Guid userId);
    Task<IReadOnlyList<AssignableRoleDto>> GetAssignableRolesAsync(Guid userId);
    Task<IReadOnlyList<AssignablePermissionDto>> GetAssignablePermissionsAsync(Guid userId);
}

public class DelegationOptionsService : IDelegationOptionsService
{
    private readonly AppDbContext _context;
    private readonly IHierarchyAuthorizationService _hierarchyAuthorization;
    private readonly IRoleMaskService _roleMaskService;

    public DelegationOptionsService(
        AppDbContext context,
        IHierarchyAuthorizationService hierarchyAuthorization,
        IRoleMaskService roleMaskService)
    {
        _context = context;
        _hierarchyAuthorization = hierarchyAuthorization;
        _roleMaskService = roleMaskService;
    }

    public async Task<IReadOnlyList<AssignableFacultyDto>> GetAssignableFacultiesAsync(Guid userId)
    {
        var scopes = await GetUserScopesAsync(userId);
        if (await HasGlobalScopeAsync(userId, scopes))
            return await _context.Faculties
                .AsNoTracking()
                .Where(faculty => faculty.Avail)
                .OrderBy(faculty => faculty.NameFaculty)
                .Select(faculty => new AssignableFacultyDto
                {
                    IdFaculty = faculty.IdFaculty,
                    NameFaculty = faculty.NameFaculty,
                    Abbreviation = faculty.Abbreviation
                })
                .ToListAsync();

        var facultyIds = await GetManagedFacultyIdsAsync(scopes);

        return await _context.Faculties
            .AsNoTracking()
            .Where(faculty => faculty.Avail && facultyIds.Contains(faculty.IdFaculty))
            .OrderBy(faculty => faculty.NameFaculty)
            .Select(faculty => new AssignableFacultyDto
            {
                IdFaculty = faculty.IdFaculty,
                NameFaculty = faculty.NameFaculty,
                Abbreviation = faculty.Abbreviation
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AssignableDepartmentDto>> GetAssignableDepartmentsAsync(Guid userId)
    {
        var scopes = await GetUserScopesAsync(userId);
        if (await HasGlobalScopeAsync(userId, scopes))
            return await GetDepartmentsQuery().ToListAsync();

        var facultyIds = scopes.Where(scope => scope.FacultyId.HasValue)
            .Select(scope => scope.FacultyId!.Value)
            .Distinct()
            .ToList();

        var departmentIds = await GetManagedDepartmentIdsAsync(scopes);

        return await GetDepartmentsQuery()
            .Where(department =>
                departmentIds.Contains(department.IdDepartment) ||
                facultyIds.Contains(department.FacultyId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AssignableGroupDto>> GetAssignableGroupsAsync(Guid userId)
    {
        var scopes = await GetUserScopesAsync(userId);
        if (await HasGlobalScopeAsync(userId, scopes))
            return await GetGroupsQuery().ToListAsync();

        var facultyIds = scopes.Where(scope => scope.FacultyId.HasValue)
            .Select(scope => scope.FacultyId!.Value)
            .Distinct()
            .ToList();

        var departmentIds = await GetManagedDepartmentIdsAsync(scopes);
        var groupIds = scopes.Where(scope => scope.GroupId.HasValue)
            .Select(scope => scope.GroupId!.Value)
            .Distinct()
            .ToList();

        return await GetGroupsQuery()
            .Where(group =>
                groupIds.Contains(group.IdGroup) ||
                departmentIds.Contains(group.DepartmentId) ||
                facultyIds.Contains(group.FacultyId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AssignableRoleDto>> GetAssignableRolesAsync(Guid userId)
    {
        var currentWeight = await _hierarchyAuthorization.GetUserMaxManagementWeightAsync(userId);
        if (currentWeight <= RoleHierarchyLevels.Student)
            return Array.Empty<AssignableRoleDto>();

        var roles = await _context.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .Select(role => new
            {
                role.IdRole,
                role.Name,
                role.IsSystem,
                role.IsStudent,
                role.CreatedInFacultyId,
                role.CreatedInDepartmentId,
                role.CreatedInGroupId
            })
            .ToListAsync();

        var result = new List<AssignableRoleDto>();
        foreach (var role in roles)
        {
            var effectiveWeight = GetRoleEffectiveWeight(role.Name, role.IsSystem, role.IsStudent);
            if (effectiveWeight >= currentWeight)
                continue;

            if (!await _hierarchyAuthorization.CanManageScopeAsync(
                    userId,
                    role.CreatedInFacultyId,
                    role.CreatedInDepartmentId,
                    role.CreatedInGroupId))
            {
                continue;
            }

            result.Add(new AssignableRoleDto
            {
                IdRole = role.IdRole,
                NameRole = role.Name,
                EffectiveWeight = effectiveWeight,
                CreatedInFacultyId = role.CreatedInFacultyId,
                CreatedInDepartmentId = role.CreatedInDepartmentId,
                CreatedInGroupId = role.CreatedInGroupId
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<AssignablePermissionDto>> GetAssignablePermissionsAsync(Guid userId)
    {
        var currentWeight = await _hierarchyAuthorization.GetUserMaxManagementWeightAsync(userId);
        if (currentWeight <= RoleHierarchyLevels.Student)
            return Array.Empty<AssignablePermissionDto>();

        var userMask = await _roleMaskService.GetUserPermissionsMaskAsync(userId);

        var permissions = await _context.Permissions
            .AsNoTracking()
            .Where(permission => permission.RequiredWeight < currentWeight)
            .OrderBy(permission => permission.BitIndex)
            .Select(permission => new AssignablePermissionDto
            {
                IdPermission = permission.IdPermission,
                Code = permission.Code,
                BitIndex = permission.BitIndex,
                RequiredWeight = permission.RequiredWeight
            })
            .ToListAsync();

        return permissions
            .Where(permission => PermissionMaskHelper.HasAll(
                userMask,
                PermissionMaskHelper.ToMask(permission.BitIndex)))
            .ToList();
    }

    private IQueryable<AssignableDepartmentDto> GetDepartmentsQuery()
    {
        return _context.Departments
            .AsNoTracking()
            .Where(department => department.Avail)
            .OrderBy(department => department.NameDepartment)
            .Select(department => new AssignableDepartmentDto
            {
                IdDepartment = department.IdDepartment,
                FacultyId = department.FacultyId,
                NameDepartment = department.NameDepartment,
                Abbreviation = department.Abbreviation
            });
    }

    private IQueryable<AssignableGroupDto> GetGroupsQuery()
    {
        return _context.StudentGroups
            .AsNoTracking()
            .Where(group => group.Avail)
            .OrderBy(group => group.GroupCode)
            .Select(group => new AssignableGroupDto
            {
                IdGroup = group.IdGroup,
                GroupCode = group.GroupCode,
                FacultyId = group.EducationalProgram.Speciality.Department.FacultyId,
                DepartmentId = group.EducationalProgram.Speciality.DepartmentId
            });
    }

    private async Task<List<UserScope>> GetUserScopesAsync(Guid userId)
    {
        return await _context.UserHierarchyAssignments
            .AsNoTracking()
            .Where(assignment => assignment.UserId == userId)
            .Select(assignment => new UserScope(
                assignment.FacultyId,
                assignment.DepartmentId,
                assignment.GroupId))
            .ToListAsync();
    }

    private async Task<List<Guid>> GetManagedFacultyIdsAsync(IReadOnlyCollection<UserScope> scopes)
    {
        var facultyIds = scopes.Where(scope => scope.FacultyId.HasValue)
            .Select(scope => scope.FacultyId!.Value)
            .ToList();

        var departmentIds = scopes.Where(scope => scope.DepartmentId.HasValue)
            .Select(scope => scope.DepartmentId!.Value)
            .ToList();

        if (departmentIds.Count > 0)
        {
            facultyIds.AddRange(await _context.Departments
                .AsNoTracking()
                .Where(department => departmentIds.Contains(department.IdDepartment))
                .Select(department => department.FacultyId)
                .ToListAsync());
        }

        var groupIds = scopes.Where(scope => scope.GroupId.HasValue)
            .Select(scope => scope.GroupId!.Value)
            .ToList();

        if (groupIds.Count > 0)
        {
            facultyIds.AddRange(await _context.StudentGroups
                .AsNoTracking()
                .Where(group => groupIds.Contains(group.IdGroup))
                .Select(group => group.EducationalProgram.Speciality.Department.FacultyId)
                .ToListAsync());
        }

        return facultyIds.Distinct().ToList();
    }

    private async Task<List<Guid>> GetManagedDepartmentIdsAsync(IReadOnlyCollection<UserScope> scopes)
    {
        var departmentIds = scopes.Where(scope => scope.DepartmentId.HasValue)
            .Select(scope => scope.DepartmentId!.Value)
            .ToList();

        var groupIds = scopes.Where(scope => scope.GroupId.HasValue)
            .Select(scope => scope.GroupId!.Value)
            .ToList();

        if (groupIds.Count > 0)
        {
            departmentIds.AddRange(await _context.StudentGroups
                .AsNoTracking()
                .Where(group => groupIds.Contains(group.IdGroup))
                .Select(group => group.EducationalProgram.Speciality.DepartmentId)
                .ToListAsync());
        }

        return departmentIds.Distinct().ToList();
    }

    private static bool IsGlobalScope(IReadOnlyCollection<UserScope> scopes)
    {
        return scopes.Any(scope =>
            !scope.FacultyId.HasValue &&
            !scope.DepartmentId.HasValue &&
            !scope.GroupId.HasValue);
    }

    private async Task<bool> HasGlobalScopeAsync(Guid userId, IReadOnlyCollection<UserScope> scopes)
    {
        if (IsGlobalScope(scopes))
            return true;

        return await _hierarchyAuthorization.GetUserMaxManagementWeightAsync(userId) >=
               RoleHierarchyLevels.SystemAdmin;
    }

    private static int GetRoleEffectiveWeight(string roleName, bool isSystem, bool isStudent)
    {
        if (isSystem && !isStudent)
            return RoleHierarchyLevels.SystemAdmin;

        if (isStudent)
            return RoleHierarchyLevels.Student;

        return RoleHierarchyCatalog.ResolveLevel(roleName);
    }

    private readonly record struct UserScope(Guid? FacultyId, Guid? DepartmentId, Guid? GroupId);
}
