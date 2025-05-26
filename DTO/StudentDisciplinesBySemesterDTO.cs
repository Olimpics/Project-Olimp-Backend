using System.Collections.Generic;

namespace OlimpBack.DTO
{
    public class StudentDisciplinesBySemesterDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string DegreeName { get; set; } = string.Empty;
        public Dictionary<int, List<BindMainDisciplineDto>> MainDisciplinesBySemester { get; set; } = new Dictionary<int, List<BindMainDisciplineDto>>();
        public Dictionary<int, List<BindAddDisciplineDto>> AdditionalDisciplinesBySemester { get; set; } = new Dictionary<int, List<BindAddDisciplineDto>>();
    }
} 