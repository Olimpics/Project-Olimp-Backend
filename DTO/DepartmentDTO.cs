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
} 