using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class StudentSelectiveDisciplinesDto
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string ThirdName { get; set; } = string.Empty;
        public string StudentName => $"{SecondName} {FirstName} {ThirdName}".Trim();
        public List<BindSelectiveDisciplineDto> AdditionalDisciplines { get; set; } = new List<BindSelectiveDisciplineDto>();
    }
    public class StudentEducationalProgramDto : StudentSelectiveDisciplinesDto
    {
        public List<MainDisciplineDto> MainDisciplines { get; set; } = new List<MainDisciplineDto>();
    }
} 
 