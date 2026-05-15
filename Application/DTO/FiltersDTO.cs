using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class FiltersDepartmentDTO
    {
        public Guid IdDepartment { get; set; }
        public string NameDepartment { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
    }

    public class SpecialityFilterDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class GroupFilterDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public int? StudentsCount { get; set; }
        public Guid? FacultyId { get; set; }
        public string? FacultyName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? Course { get; set; }
        public Guid? DegreeId { get; set; }
        public string? DegreeName { get; set; }
    }

    public class DisciplineFiltersDto
    {
        public List<string>? Faculties { get; set; }
        public List<int>? Courses { get; set; }
        public bool? IsEvenSemester { get; set; }
        public List<Guid>? DegreeLevelIds { get; set; }
    }
    public class NotificationTemplateFilterDto
    {
        public Guid IdNotificationTemplates { get; set; }
        public string NotificationType { get; set; } = null!;
    }

    public class EducationalProgramFilterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class SelectiveDisciplineFilterQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public Guid? CatalogYearId { get; set; }
    }
}
