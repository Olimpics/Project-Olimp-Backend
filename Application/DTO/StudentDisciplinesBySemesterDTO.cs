using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class StudentDisciplinesBySemesterDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string DegreeName { get; set; } = string.Empty;
        public Dictionary<int, List<MainDisciplineDto>> MainDisciplinesBySemester { get; set; } = new Dictionary<int, List<MainDisciplineDto>>();
        public Dictionary<int, List<BindAddDisciplineDto>> AdditionalDisciplinesBySemester { get; set; } = new Dictionary<int, List<BindAddDisciplineDto>>();
    }
} 