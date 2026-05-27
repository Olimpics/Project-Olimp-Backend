namespace OlimpBack.Application.Permissions;

/// <summary>
/// Role hierarchy for delegation.
/// Prefer DB flags is_student / is_admin; role name is fallback until migration is applied.
/// </summary>
public static class RoleHierarchyLevels
{
    public const int Student = 0;
    public const int Curator = 20;
    public const int FacultyAdmin = 50;
    public const int SystemAdmin = 100;
}

public static class RoleHierarchyCatalog
{
    public static readonly IReadOnlyDictionary<string, int> ByRoleName =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["Student"] = RoleHierarchyLevels.Student,
            ["Студент"] = RoleHierarchyLevels.Student,

            ["Curator"] = RoleHierarchyLevels.Curator,
            ["Куратор"] = RoleHierarchyLevels.Curator,
            ["Dean"] = RoleHierarchyLevels.FacultyAdmin,

            ["FacultyAdmin"] = RoleHierarchyLevels.FacultyAdmin,
            ["Faculty Admin"] = RoleHierarchyLevels.FacultyAdmin,

            ["Administrator"] = RoleHierarchyLevels.SystemAdmin,
            ["Admin"] = RoleHierarchyLevels.SystemAdmin,
            ["SystemAdmin"] = RoleHierarchyLevels.SystemAdmin,
        };

    public static int ResolveLevel(string roleName, int? storedLevel = null)
    {
        if (storedLevel is > 0)
            return storedLevel.Value;

        return ByRoleName.TryGetValue(roleName, out var level)
            ? level
            : RoleHierarchyLevels.Student;
    }

    public static bool IsStudentRole(RoleKindSnapshot role) =>
        role.IsStudent || (!role.IsAdmin && ResolveLevel(role.Name) == RoleHierarchyLevels.Student);

    public static bool IsAdminRole(RoleKindSnapshot role) =>
        role.IsAdmin || ResolveLevel(role.Name) >= RoleHierarchyLevels.SystemAdmin;

    public static bool IsStaffRole(RoleKindSnapshot role) =>
        !IsStudentRole(role) || IsAdminRole(role);

    public static bool IsStudentRole(string roleName) =>
        ResolveLevel(roleName) == RoleHierarchyLevels.Student;

    public static int GetEffectiveGranterLevel(IEnumerable<RoleKindSnapshot> granterRoles)
    {
        var roles = granterRoles.ToList();
        if (roles.Count == 0)
            return RoleHierarchyLevels.Student;

        if (roles.Any(IsAdminRole))
            return RoleHierarchyLevels.SystemAdmin;

        var staffLevels = roles
            .Where(r => !IsStudentRole(r))
            .Select(r => ResolveLevel(r.Name))
            .ToList();

        return staffLevels.Count == 0
            ? RoleHierarchyLevels.Student
            : staffLevels.Max();
    }

    public static int GetEffectiveGranterLevel(IEnumerable<string> granterRoleNames)
    {
        var snapshots = granterRoleNames.Select(name => new RoleKindSnapshot(false, false, name));
        return GetEffectiveGranterLevel(snapshots);
    }

    public static bool CanAssignUserRole(IEnumerable<RoleKindSnapshot> granterRoles, RoleKindSnapshot targetRole)
    {
        var granterLevel = GetEffectiveGranterLevel(granterRoles);

        if (granterLevel == RoleHierarchyLevels.Student)
            return false;

        if (IsStudentRole(targetRole))
            return granterRoles.Any(IsAdminRole) || granterLevel >= RoleHierarchyLevels.SystemAdmin;

        if (granterRoles.Any(IsAdminRole) || granterLevel >= RoleHierarchyLevels.SystemAdmin)
            return true;

        return ResolveLevel(targetRole.Name) < granterLevel;
    }

    public static bool CanAssignUserRole(IEnumerable<string> granterRoleNames, string targetRoleName)
    {
        var granterSnapshots = granterRoleNames.Select(n => new RoleKindSnapshot(false, false, n));
        var targetSnapshot = new RoleKindSnapshot(false, false, targetRoleName);
        return CanAssignUserRole(granterSnapshots, targetSnapshot);
    }

    public static bool CanManageRolePermissions(IEnumerable<RoleKindSnapshot> granterRoles, RoleKindSnapshot targetRole) =>
        CanAssignUserRole(granterRoles, targetRole);
}
