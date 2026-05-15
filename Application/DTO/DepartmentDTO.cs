using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class DepartmentDto
    {
        public Guid IdDepartment { get; set; }
        public Guid FacultyId { get; set; }
        public string NameDepartment { get; set; } = null!;
        public string? Abbreviation { get; set; }
        public string? FacultyName { get; set; }
    }

    public class CreateDepartmentDto
    {
        public Guid FacultyId { get; set; }
        public string NameDepartment { get; set; } = null!;
        public string? Abbreviation { get; set; }
    }

    public class UpdateDepartmentDto : CreateDepartmentDto
    {
        public Guid IdDepartment { get; set; }
    }

    public class DepartmentQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public List<Guid>? FacultyIds { get; set; }
        public string? Search { get; set; }
        public int SortOrder { get; set; } = 0;
    }
}
