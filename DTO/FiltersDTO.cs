using System.Collections.Generic;

namespace OlimpBack.DTO
{
    public class FiltersDepartmentDTO
    {
        public int IdDepartment { get; set; }
        public string NameDepartment { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
        public string FacultyName { get; set; }
    }

    public class SpecialityFilterDto
    {
        public int Id { get; set; }
        public string NameEP { get; set; }
    }

    public class GroupFilterDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? StudentsCount { get; set; }
        public int? FacultyId { get; set; }
        public string? FacultyName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? Course { get; set; }
        public int? DegreeId { get; set; }
        public string? DegreeName { get; set; }
    }

    public class NotificationTemplateFilterDto
    {
        public int IdNotificationTemplates { get; set; }
        public string NotificationType { get; set; } = null!;
    }
}
