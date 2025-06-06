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
        public string Name { get; set; }
    }

    public class GroupFilterDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? StudentsCount { get; set; }
    }
}
