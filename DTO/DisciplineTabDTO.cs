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

    public class FullDisciplineDto
    {
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public string Department { get; set; } = null!;
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public sbyte? AddSemestr { get; set; }
        public string? Recomend { get; set; }
        public string? Teacher { get; set; }
        public string? Prerequisites { get; set; }
        public string? DegreeLevel { get; set; }
        public bool IsAvailable { get; set; }
        public int CountOfPeople { get; set; }
    }

} 