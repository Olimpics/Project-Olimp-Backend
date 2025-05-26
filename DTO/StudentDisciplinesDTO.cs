using System.Collections.Generic;

namespace OlimpBack.DTO
{
    public class StudentDisciplinesDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<BindMainDisciplineDto> MainDisciplines { get; set; } = new List<BindMainDisciplineDto>();
        public List<BindAddDisciplineDto> AdditionalDisciplines { get; set; } = new List<BindAddDisciplineDto>();
    }
} 