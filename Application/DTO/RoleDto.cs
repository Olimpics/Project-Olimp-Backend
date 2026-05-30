using System;

namespace OlimpBack.Application.DTO
{
    public class RoleDto
    {
        public Guid IdRole { get; set; }
        public string NameRole { get; set; } = null!;
        public Guid? ParentRoleId { get; set; }
        public bool IsSystem { get; set; }
        public bool IsStudent { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? CreatedInFacultyId { get; set; }
        public Guid? CreatedInDepartmentId { get; set; }
        public Guid? CreatedInGroupId { get; set; }
    }

}
