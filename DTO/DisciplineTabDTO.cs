using OlimpBack.Models;
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

    public class SimpleDisciplineDto
    {
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
    }

    public class FullDisciplineDto
    {
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public sbyte? AddSemestr { get; set; }
        public string DegreeLevelName { get; set; }
        public bool IsAvailable { get; set; }
        public int CountOfPeople { get; set; }
    }

    public class FullForAdminDisciplineDto
    {
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public sbyte? AddSemestr { get; set; }
        public string DegreeLevelName { get; set; }
        public int CountOfPeople { get; set; }
    }
    public class AddDisciplineBindDto
    {
        public int StudentId { get; set; }
        public int DisciplineId { get; set; }
        public int Semester { get; set; }
    }

    public class DisciplineAvailabilityContext
    {
        public Student Student { get; set; } = null!;
        public int CurrentCourse { get; set; }
        public string? FacultyAbbreviation { get; set; }
        public HashSet<int> BoundDisciplineIds { get; set; } = new();
        public Dictionary<int, int> DisciplineCounts { get; set; } = new();
    }

    public class FullDisciplineWithDetailsDto
    {
        // Basic discipline info
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
        public int FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public sbyte? AddSemestr { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        
        // Details from AddDetail
        public string DepartmentName { get; set; } = null!;
        public string? Teacher { get; set; }
        public string? Recomend { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Determination { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }
        public string? AdditionaLiterature { get; set; }
        public string TypesOfTraining { get; set; } = null!;
        public string TypeOfControll { get; set; } = null!;
    }

    public class DisciplineFiltersDto
    {
        public List<string>? Faculties { get; set; }
        public List<int>? Courses { get; set; }
        public bool? IsEvenSemester { get; set; }
        public List<int>? DegreeLevelIds { get; set; }
    }
}