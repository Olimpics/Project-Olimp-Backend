using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Permissions;

public class HierarchyAuthorizationService : IHierarchyAuthorizationService
{
    private const int SystemManagementWeight = 100;

    private readonly AppDbContext _context;

    public HierarchyAuthorizationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetUserMaxManagementWeightAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (await IsSystemUserAsync(userId, cancellationToken))
            return SystemManagementWeight;

        return await _context.UserHierarchyAssignments
            .AsNoTracking()
            .Where(assignment => assignment.UserId == userId)
            .Select(assignment => (int?)assignment.HierarchyNode.ManagementWeight)
            .MaxAsync(cancellationToken) ?? 0;
    }

    public async Task<bool> CanGrantPermissionAsync(
        Guid granterUserId,
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        var requiredWeight = await _context.Permissions
            .AsNoTracking()
            .Where(permission => permission.IdPermission == permissionId)
            .Select(permission => (int?)permission.RequiredWeight)
            .FirstOrDefaultAsync(cancellationToken);

        if (!requiredWeight.HasValue)
            return false;

        var granterWeight = await GetUserMaxManagementWeightAsync(granterUserId, cancellationToken);
        return granterWeight >= requiredWeight.Value;
    }

    public async Task<bool> CanManageScopeAsync(
        Guid granterUserId,
        Guid? facultyId,
        Guid? departmentId,
        Guid? groupId,
        CancellationToken cancellationToken = default)
    {
        if (await IsSystemUserAsync(granterUserId, cancellationToken))
            return true;

        var scope = await ResolveScopeAsync(facultyId, departmentId, groupId, cancellationToken);
        if (scope == null)
            return false;

        var assignments = await _context.UserHierarchyAssignments
            .AsNoTracking()
            .Where(assignment => assignment.UserId == granterUserId)
            .ToListAsync(cancellationToken);

        return assignments.Any(assignment => ScopeCovers(assignment, scope.Value));
    }

    public async Task<bool> CanAssignHierarchyNodeAsync(
        Guid granterUserId,
        Guid targetHierarchyNodeId,
        Guid? facultyId,
        Guid? departmentId,
        Guid? groupId,
        CancellationToken cancellationToken = default)
    {
        if (await IsSystemUserAsync(granterUserId, cancellationToken))
            return true;

        var targetNode = await _context.HierarchyNodes
            .AsNoTracking()
            .Where(node => node.IdNode == targetHierarchyNodeId)
            .Select(node => new { node.IdNode, node.TreeId, node.ManagementWeight })
            .FirstOrDefaultAsync(cancellationToken);

        if (targetNode == null)
            return false;

        var scope = await ResolveScopeAsync(facultyId, departmentId, groupId, cancellationToken);
        if (scope == null)
            return false;

        var assignments = await _context.UserHierarchyAssignments
            .AsNoTracking()
            .Include(assignment => assignment.HierarchyNode)
            .Where(assignment =>
                assignment.UserId == granterUserId &&
                assignment.HierarchyNode.TreeId == targetNode.TreeId &&
                assignment.HierarchyNode.ManagementWeight > targetNode.ManagementWeight)
            .ToListAsync(cancellationToken);

        foreach (var assignment in assignments)
        {
            if (!ScopeCovers(assignment, scope.Value))
                continue;

            if (await IsAncestorAsync(assignment.HierarchyNodeId, targetNode.IdNode, cancellationToken))
                return true;
        }

        return false;
    }

    private async Task<bool> IsSystemUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .AnyAsync(
                userRole => userRole.UserId == userId &&
                            userRole.Role.IsSystem &&
                            !userRole.Role.IsStudent,
                cancellationToken);
    }

    private async Task<bool> IsAncestorAsync(
        Guid ancestorNodeId,
        Guid descendantNodeId,
        CancellationToken cancellationToken)
    {
        var currentParentId = await _context.HierarchyNodes
            .AsNoTracking()
            .Where(node => node.IdNode == descendantNodeId)
            .Select(node => node.ParentId)
            .FirstOrDefaultAsync(cancellationToken);

        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == ancestorNodeId)
                return true;

            currentParentId = await _context.HierarchyNodes
                .AsNoTracking()
                .Where(node => node.IdNode == currentParentId.Value)
                .Select(node => node.ParentId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return false;
    }

    private async Task<ResolvedScope?> ResolveScopeAsync(
        Guid? facultyId,
        Guid? departmentId,
        Guid? groupId,
        CancellationToken cancellationToken)
    {
        if (groupId.HasValue)
        {
            var groupScope = await _context.StudentGroups
                .AsNoTracking()
                .Where(group => group.IdGroup == groupId.Value)
                .Select(group => (ResolvedScope?)new ResolvedScope(
                    group.EducationalProgram.Speciality.Department.FacultyId,
                    group.EducationalProgram.Speciality.DepartmentId,
                    group.IdGroup))
                .FirstOrDefaultAsync(cancellationToken);

            if (groupScope == null)
                return null;

            if (facultyId.HasValue && groupScope.Value.FacultyId != facultyId.Value)
                return null;

            if (departmentId.HasValue && groupScope.Value.DepartmentId != departmentId.Value)
                return null;

            return groupScope.Value;
        }

        if (departmentId.HasValue)
        {
            var departmentScope = await _context.Departments
                .AsNoTracking()
                .Where(department => department.IdDepartment == departmentId.Value)
                .Select(department => (ResolvedScope?)new ResolvedScope(
                    department.FacultyId,
                    department.IdDepartment,
                    null))
                .FirstOrDefaultAsync(cancellationToken);

            if (departmentScope == null)
                return null;

            if (facultyId.HasValue && departmentScope.Value.FacultyId != facultyId.Value)
                return null;

            return departmentScope.Value;
        }

        if (facultyId.HasValue)
        {
            var exists = await _context.Faculties
                .AsNoTracking()
                .AnyAsync(faculty => faculty.IdFaculty == facultyId.Value, cancellationToken);

            return exists
                ? new ResolvedScope(facultyId.Value, null, null)
                : null;
        }

        return new ResolvedScope(null, null, null);
    }

    private static bool ScopeCovers(UserHierarchyAssignment assignment, ResolvedScope scope)
    {
        if (!assignment.FacultyId.HasValue &&
            !assignment.DepartmentId.HasValue &&
            !assignment.GroupId.HasValue)
        {
            return true;
        }

        if (scope.GroupId.HasValue)
        {
            return assignment.GroupId == scope.GroupId ||
                   assignment.DepartmentId == scope.DepartmentId ||
                   assignment.FacultyId == scope.FacultyId;
        }

        if (scope.DepartmentId.HasValue)
        {
            return assignment.DepartmentId == scope.DepartmentId ||
                   assignment.FacultyId == scope.FacultyId;
        }

        if (scope.FacultyId.HasValue)
            return assignment.FacultyId == scope.FacultyId;

        return false;
    }

    private readonly record struct ResolvedScope(Guid? FacultyId, Guid? DepartmentId, Guid? GroupId);
}
