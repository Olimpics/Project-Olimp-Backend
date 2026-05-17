using System;
using OlimpBack.Models;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class DisciplineTabResponseDto
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string ThirdName { get; set; } = string.Empty;
        public string StudentName => $"{SecondName} {FirstName} {ThirdName}".Trim();
        public int CurrentCourse { get; set; }
        public bool IsEvenSemester { get; set; }
        public List<SimpleDisciplineDto> Disciplines { get; set; } = new List<SimpleDisciplineDto>();
    }

    public class SimpleDisciplineDto
    {
        public Guid IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
    }

    public class FullDisciplineDto
    {
        public Guid IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public Guid FacultyId { get; set; }
        public string FacultyAbbreviation { get; set; } = null!;
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public int? MaxCountPeople { get; set; }
        public List<int> Courses { get; set; } = new List<int>();
        public bool? IsEven { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public int CountOfPeople { get; set; }
        public Guid? CatalogId { get; set; }
        public Guid? ApprovalStatusId { get; set; }
        public Guid? TypeOfControlId { get; set; }
    }

   
    public class SelectiveDisciplineBindDto
    {
        public Guid StudentId { get; set; }
        public Guid DisciplineId { get; set; }
        public int Semestr { get; set; }
        public int Loans { get; set; }
    }

    public class DisciplineAvailabilityContext
    {
        public Student Student { get; set; } = null!;
        public int CurrentCourse { get; set; }
        public string? FacultyAbbreviation { get; set; }
        public HashSet<Guid> BoundDisciplineIds { get; set; } = new();
        public Dictionary<Guid, int> DisciplineCounts { get; set; } = new();
    }
    public class GetDisciplinesBySemesterQueryDto
    {
        public Guid StudentId { get; set; }
        public bool IsEvenSemester { get; set; }
    }
    public class FullDisciplineWithDetailsDto
    {
        // Basic discipline info
        public Guid IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;

        public string? FacultyAbbreviation { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public List<int> Courses { get; set; } = new List<int>();
        public bool? IsEven { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        public Guid? CatalogId { get; set; }
        public Guid? ApprovalStatusId { get; set; }
        public Guid? TypeOfControlId { get; set; }
        
        // Details from SelectiveDetail
        public string? NameSelectiveDisciplinesEng { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string? Teacher { get; set; }
        public string? Recomend { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Provision  { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }
        public string? DisciplineTopics { get; set; }
        public string TypesOfTraining { get; set; } = null!;
        public string TypeOfControl { get; set; } = null!;
    }



    /// <summary>
    /// Query parameters for GetAllDisciplinesWithAvailability.
    /// </summary>
    public class GetAllDisciplinesWithAvailabilityQueryDto
    {
        public Guid StudentId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public bool OnlyAvailable { get; set; } = false;
        public string? Search { get; set; }

        public Guid? CatalogId { get; set; }
        public List<Guid>? Faculties { get; set; }
        public List<int>? Courses { get; set; }
        public List<Guid>? DegreeLevelIds { get; set; }
        public List<Guid>? TypeOfControlIds { get; set; }
        public List<Guid>? ApprovalStatusIds { get; set; }

        public bool? IsEvenSemester { get; set; }
        public int SortOrder { get; set; } = 0;
    }

}
