namespace OlimpBack.DTO
{
    public class DepartmentDto
    {
        public int IdDepartment { get; set; }
        public int FacultyId { get; set; }
        public string NameDepartment { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
        public string FacultyName { get; set; } = null!;
    }

    public class CreateDepartmentDto
    {
        public int FacultyId { get; set; }
        public string NameDepartment { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
    }

    public class UpdateDepartmentDto : CreateDepartmentDto
    {
        public int IdDepartment { get; set; }
    }

    public class DepartmentQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? FacultyIds { get; set; }
        public string? Search { get; set; }
        public int SortOrder { get; set; } = 0;
    }
} 