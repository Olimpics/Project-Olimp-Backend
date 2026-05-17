using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class StudentDisciplinesBySemesterDTO
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string ThirdName { get; set; } = string.Empty;
        public string StudentName => $"{SecondName} {FirstName} {ThirdName}".Trim();
        public string DegreeName { get; set; } = string.Empty;
        public Dictionary<int, List<MainDisciplineDto>> MainDisciplinesBySemester { get; set; } = new Dictionary<int, List<MainDisciplineDto>>();
        public Dictionary<int, List<BindSelectiveDisciplineDto>> AdditionalDisciplinesBySemester { get; set; } = new Dictionary<int, List<BindSelectiveDisciplineDto>>();
    }
} 
 