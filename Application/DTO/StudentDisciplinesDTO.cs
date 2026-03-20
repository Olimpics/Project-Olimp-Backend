using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class StudentAddDisciplinesDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<BindAddDisciplineDto> AdditionalDisciplines { get; set; } = new List<BindAddDisciplineDto>();
    }
    public class StudentEducationalProgramDto : StudentAddDisciplinesDto
    {
        public List<BindMainDisciplineDto> MainDisciplines { get; set; } = new List<BindMainDisciplineDto>();
    }
} 