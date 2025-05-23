namespace OlimpBack.DTO
{
    public class EducationalDegreeDto
    {
        public int IdEducationalDegree { get; set; }
        public string NameEducationalDegreec { get; set; } = null!;
        public int StudentsCount { get; set; }
    }

    public class CreateEducationalDegreeDto
    {
        public string NameEducationalDegreec { get; set; } = null!;
    }

    public class UpdateEducationalDegreeDto : CreateEducationalDegreeDto
    {
        public int IdEducationalDegree { get; set; }
    }

}
