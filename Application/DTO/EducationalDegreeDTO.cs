using System;
using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    public class EducationalDegreeDto
    {
        public Guid IdEducationalDegree { get; set; }
        public string NameEducationalDegree { get; set; } = null!;
        public int StudentsCount { get; set; }
    }

    public class CreateEducationalDegreeDto
    {
        public string NameEducationalDegree { get; set; } = null!;
    }

    public class UpdateEducationalDegreeDto : CreateEducationalDegreeDto
    {
        public Guid IdEducationalDegree { get; set; }
    }

}
