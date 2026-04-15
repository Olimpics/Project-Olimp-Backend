using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class FiltersDepartmentDTO
    {
        public int IdDepartment { get; set; }
        public string NameDepartment { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
    }

    public class SpecialityFilterDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
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

    public class DisciplineFiltersDto
    {
        public List<string>? Faculties { get; set; }
        public List<int>? Courses { get; set; }
        public bool? IsEvenSemester { get; set; }
        public List<int>? DegreeLevelIds { get; set; }
    }
    public class NotificationTemplateFilterDto
    {
        public int IdNotificationTemplates { get; set; }
        public string NotificationType { get; set; } = null!;
    }

    public class EducationalProgramFilterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class AddDisciplineFilterQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public int? CatalogYearId { get; set; }
    }
}
