using System;

namespace OlimpBack.Application.DTO
{
    public class ApprovalDto
    {
        public Guid IdApproval { get; set; }
        public string AppovalStatus { get; set; } = null!;
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
        public int ApprobalLevel { get; set; }
    }

    public class CreateApprovalDto
    {
        public string AppovalStatus { get; set; } = null!;
        public Guid RoleId { get; set; }
        public int ApprobalLevel { get; set; }
    }

    public class UpdateApprovalDto : CreateApprovalDto
    {
        public Guid IdApproval { get; set; }
    }
}
