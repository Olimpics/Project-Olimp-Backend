using System.Collections.Generic;

namespace OlimpBack.DTO
{
    public class DisciplineTabResponseDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CurrentCourse { get; set; }
        public bool IsEvenSemester { get; set; }
        public List<SimpleDisciplineDto> Disciplines { get; set; } = new List<SimpleDisciplineDto>();
    }
} 