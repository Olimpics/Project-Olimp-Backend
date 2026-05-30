using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Role
{
    public string Name { get; set; } = null!;

    public long? PermissionsMask { get; set; }

    public Guid? ParentRoleId { get; set; }

    public Guid IdRole { get; set; }

    public bool IsSystem { get; set; }

    public bool IsStudent { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public Guid? CreatedInFacultyId { get; set; }

    public Guid? CreatedInDepartmentId { get; set; }

    public Guid? CreatedInGroupId { get; set; }

    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public virtual User? CreatedByUser { get; set; }

    public virtual Department? CreatedInDepartment { get; set; }

    public virtual Faculty? CreatedInFaculty { get; set; }

    public virtual StudentGroup? CreatedInGroup { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
