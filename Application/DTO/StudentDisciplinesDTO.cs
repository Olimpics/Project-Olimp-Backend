using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class StudentSelectiveDisciplinesDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<BindSelectiveDisciplineDto> AdditionalDisciplines { get; set; } = new List<BindSelectiveDisciplineDto>();
    }
    public class StudentEducationalProgramDto : StudentSelectiveDisciplinesDto
    {
        public List<MainDisciplineDto> MainDisciplines { get; set; } = new List<MainDisciplineDto>();
    }
} 