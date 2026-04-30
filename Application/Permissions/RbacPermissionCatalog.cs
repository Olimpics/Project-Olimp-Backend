namespace OlimpBack.Application.Permissions;

public sealed record PermissionDefinition(string Code, RbacPermissions Permission)
{
    public int BitIndex => (int)Permission;
}

public enum RbacPermissions
{
    UsersCreate = 0,
    UsersRead = 1,
    UsersUpdate = 2,
    UsersDelete = 3,

    RolesCreate = 4,
    RolesRead = 5,
    RolesUpdate = 6,
    RolesDelete = 7,

    PermissionsCreate = 8,
    PermissionsRead = 9,
    PermissionsUpdate = 10,
    PermissionsDelete = 11,

    RolePermissionsCreate = 12,
    RolePermissionsRead = 13,
    RolePermissionsUpdate = 14,
    RolePermissionsDelete = 15,

    StudentsCreate = 16,
    StudentsRead = 17,
    StudentsUpdate = 18,
    StudentsDelete = 19,

    FacultiesCreate = 20,
    FacultiesRead = 21,
    FacultiesUpdate = 22,
    FacultiesDelete = 23,

    DepartmentsCreate = 24,
    DepartmentsRead = 25,
    DepartmentsUpdate = 26,
    DepartmentsDelete = 27,

    GroupsCreate = 28,
    GroupsRead = 29,
    GroupsUpdate = 30,
    GroupsDelete = 31,

    EducationalProgramsCreate = 32,
    EducationalProgramsRead = 33,
    EducationalProgramsUpdate = 34,
    EducationalProgramsDelete = 35,

    EducationalDegreesCreate = 36,
    EducationalDegreesRead = 37,
    EducationalDegreesUpdate = 38,
    EducationalDegreesDelete = 39,

    MainDisciplinesCreate = 40,
    MainDisciplinesRead = 41,
    MainDisciplinesUpdate = 42,
    MainDisciplinesDelete = 43,

    DisciplineCreate = 44,
    DisciplineRead = 45,
    DisciplineUpdate = 46,
    DisciplineDelete = 47,
    DisciplineTeachersPermission = 48,

    DisciplineChoicePeriodsCreate = 49,
    DisciplineChoicePeriodsRead = 50,
    DisciplineChoicePeriodsUpdate = 51,
    DisciplineChoicePeriodsDelete = 52,

    NotificationsCreate = 53,
    NotificationsRead = 54,
    NotificationsUpdate = 55,
    NotificationsDelete = 56,

    ParametersCreate = 57,
    ParametersRead = 58,
    ParametersUpdate = 59,
    ParametersDelete = 60
}

public static class RbacPermissionCatalog
{
    public static readonly IReadOnlyList<PermissionDefinition> All = new[]
    {
        new PermissionDefinition("users.create", RbacPermissions.UsersCreate),
        new PermissionDefinition("users.read", RbacPermissions.UsersRead),
        new PermissionDefinition("users.update", RbacPermissions.UsersUpdate),
        new PermissionDefinition("users.delete", RbacPermissions.UsersDelete),

        new PermissionDefinition("roles.create", RbacPermissions.RolesCreate),
        new PermissionDefinition("roles.read", RbacPermissions.RolesRead),
        new PermissionDefinition("roles.update", RbacPermissions.RolesUpdate),
        new PermissionDefinition("roles.delete", RbacPermissions.RolesDelete),

        new PermissionDefinition("permissions.create", RbacPermissions.PermissionsCreate),
        new PermissionDefinition("permissions.read", RbacPermissions.PermissionsRead),
        new PermissionDefinition("permissions.update", RbacPermissions.PermissionsUpdate),
        new PermissionDefinition("permissions.delete", RbacPermissions.PermissionsDelete),

        new PermissionDefinition("rolePermissions.create", RbacPermissions.RolePermissionsCreate),
        new PermissionDefinition("rolePermissions.read", RbacPermissions.RolePermissionsRead),
        new PermissionDefinition("rolePermissions.update", RbacPermissions.RolePermissionsUpdate),
        new PermissionDefinition("rolePermissions.delete", RbacPermissions.RolePermissionsDelete),

        new PermissionDefinition("students.create", RbacPermissions.StudentsCreate),
        new PermissionDefinition("students.read", RbacPermissions.StudentsRead),
        new PermissionDefinition("students.update", RbacPermissions.StudentsUpdate),
        new PermissionDefinition("students.delete", RbacPermissions.StudentsDelete),

        new PermissionDefinition("faculties.create", RbacPermissions.FacultiesCreate),
        new PermissionDefinition("faculties.read", RbacPermissions.FacultiesRead),
        new PermissionDefinition("faculties.update", RbacPermissions.FacultiesUpdate),
        new PermissionDefinition("faculties.delete", RbacPermissions.FacultiesDelete),

        new PermissionDefinition("departments.create", RbacPermissions.DepartmentsCreate),
        new PermissionDefinition("departments.read", RbacPermissions.DepartmentsRead),
        new PermissionDefinition("departments.update", RbacPermissions.DepartmentsUpdate),
        new PermissionDefinition("departments.delete", RbacPermissions.DepartmentsDelete),

        new PermissionDefinition("groups.create", RbacPermissions.GroupsCreate),
        new PermissionDefinition("groups.read", RbacPermissions.GroupsRead),
        new PermissionDefinition("groups.update", RbacPermissions.GroupsUpdate),
        new PermissionDefinition("groups.delete", RbacPermissions.GroupsDelete),

        new PermissionDefinition("educationalPrograms.create", RbacPermissions.EducationalProgramsCreate),
        new PermissionDefinition("educationalPrograms.read", RbacPermissions.EducationalProgramsRead),
        new PermissionDefinition("educationalPrograms.update", RbacPermissions.EducationalProgramsUpdate),
        new PermissionDefinition("educationalPrograms.delete", RbacPermissions.EducationalProgramsDelete),

        new PermissionDefinition("educationalDegrees.create", RbacPermissions.EducationalDegreesCreate),
        new PermissionDefinition("educationalDegrees.read", RbacPermissions.EducationalDegreesRead),
        new PermissionDefinition("educationalDegrees.update", RbacPermissions.EducationalDegreesUpdate),
        new PermissionDefinition("educationalDegrees.delete", RbacPermissions.EducationalDegreesDelete),

        new PermissionDefinition("mainDisciplines.create", RbacPermissions.MainDisciplinesCreate),
        new PermissionDefinition("mainDisciplines.read", RbacPermissions.MainDisciplinesRead),
        new PermissionDefinition("mainDisciplines.update", RbacPermissions.MainDisciplinesUpdate),
        new PermissionDefinition("mainDisciplines.delete", RbacPermissions.MainDisciplinesDelete),

        new PermissionDefinition("discipline.create", RbacPermissions.DisciplineCreate),
        new PermissionDefinition("discipline.read", RbacPermissions.DisciplineRead),
        new PermissionDefinition("discipline.update", RbacPermissions.DisciplineUpdate),
        new PermissionDefinition("discipline.delete", RbacPermissions.DisciplineDelete),
        new PermissionDefinition("discipline.teachersPermission", RbacPermissions.DisciplineTeachersPermission),

        new PermissionDefinition("disciplineChoicePeriods.create", RbacPermissions.DisciplineChoicePeriodsCreate),
        new PermissionDefinition("disciplineChoicePeriods.read", RbacPermissions.DisciplineChoicePeriodsRead),
        new PermissionDefinition("disciplineChoicePeriods.update", RbacPermissions.DisciplineChoicePeriodsUpdate),
        new PermissionDefinition("disciplineChoicePeriods.delete", RbacPermissions.DisciplineChoicePeriodsDelete),

        new PermissionDefinition("notifications.create", RbacPermissions.NotificationsCreate),
        new PermissionDefinition("notifications.read", RbacPermissions.NotificationsRead),
        new PermissionDefinition("notifications.update", RbacPermissions.NotificationsUpdate),
        new PermissionDefinition("notifications.delete", RbacPermissions.NotificationsDelete),

        new PermissionDefinition("parameters.create", RbacPermissions.ParametersCreate),
        new PermissionDefinition("parameters.read", RbacPermissions.ParametersRead),
        new PermissionDefinition("parameters.update", RbacPermissions.ParametersUpdate),
        new PermissionDefinition("parameters.delete", RbacPermissions.ParametersDelete),
    };
}
