using System;

namespace OlimpBack.Application.DTO
{

    public class CreatePermissionDto
    {
        public string TypePermission { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public int BitIndex { get; set; }
    }

    public class UpdatePermissionDto
    {
        public Guid IdPermissions { get; set; }
        public string TypePermission { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public int BitIndex { get; set; }
    }

    public class BindRolePermissionDto
    {
        public Guid IdBindRolePermission { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public Guid PermissionId { get; set; }
        public string TypePermission { get; set; } = null!;
        public string TableName { get; set; } = null!;
    }

    public class CreateBindRolePermissionDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }

    public class UpdateBindRolePermissionDto
    {
        public Guid IdBindRolePermission { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
} 
 