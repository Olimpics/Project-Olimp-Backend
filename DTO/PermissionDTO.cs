namespace OlimpBack.DTO
{

    public class CreatePermissionDto
    {
        public string NamePermission { get; set; }
    }

    public class UpdatePermissionDto
    {
        public int IdPermissions { get; set; }
        public string NamePermission { get; set; }
    }

    public class BindRolePermissionDto
    {
        public int IdBindRolePermission { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int PermissionId { get; set; }
        public string TypePermission { get; set; }
        public string TableName { get; set; }
    }

    public class CreateBindRolePermissionDto
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }

    public class UpdateBindRolePermissionDto
    {
        public int IdBindRolePermission { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }
} 