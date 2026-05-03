using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    public class EducationalDegreeDto
    {
        public int IdEducationalDegree { get; set; }
        public string NameEducationalDegree { get; set; } = null!;
        public int StudentsCount { get; set; }
    }

    public class CreateEducationalDegreeDto
    {
        public string NameEducationalDegree { get; set; } = null!;
    }

    public class UpdateEducationalDegreeDto : CreateEducationalDegreeDto
    {
        public int IdEducationalDegree { get; set; }
    }

}
