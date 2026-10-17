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
        public RecommendedDto? Recommended { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Provision  { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }
        public List<string>? DisciplineTopics { get; set; }
        public string TypesOfTraining { get; set; } = null!;
        public string TypeOfControl { get; set; } = null!;
        
        // New fields
        public string? Feedback { get; set; }
        public bool IsForseChange { get; set; }
        public string? NameDock { get; set; }
        public List<string>? Keys { get; set; }
        public string? ApprovalStatus { get; set; }
        public bool NeedFix { get; set; }
        public int YearStart { get; set; }
        public int YearEnd { get; set; }

        public class RecommendedDto
        {
            public List<string> Branches { get; set; } = new();
            public List<string> Specialties { get; set; } = new();

            [System.Text.Json.Serialization.JsonPropertyName("EducationalPrograms")]
            public List<string> EducationalProgram { get; set; } = new();
        }
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

    public class SimilarDisciplineGroupDto
    {
        public string GroupName { get; set; } = null!;
        public List<SimilarDisciplineDto> Disciplines { get; set; } = new();
    }

    public class SimilarDisciplineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int YearStart { get; set; }
        public int YearEnd { get; set; }
    }

    // ── Student's own selective disciplines ──────────────────────────────────

    /// <summary>
    /// Query parameters to fetch a student's own bound selective disciplines.
    /// </summary>
    public class StudentOwnSelectiveDisciplinesQueryDto
    {
        public Guid StudentId { get; set; }
        /// <summary>Optional filter: limit to specific catalog year IDs (CatalogYearsSelective).</summary>
        public List<Guid>? CatalogYearIds { get; set; }
    }

    /// <summary>
    /// A single entry in a student's bound selective discipline list.
    /// </summary>
    public class StudentOwnSelectiveDisciplineItemDto
    {
        public Guid BindId { get; set; }
        public Guid SelectiveDisciplineId { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public int Semestr { get; set; }
        public int? Loans { get; set; }
        public int Grade { get; set; }
        public bool InProcess { get; set; }
        public bool IsRejected { get; set; }
        public Guid CatalogYearId { get; set; }
        public int CatalogYearStart { get; set; }
        public int CatalogYearEnd { get; set; }
        public string? DepartmentName { get; set; }
        public string? FacultyAbbreviation { get; set; }
    }

    /// <summary>
    /// Wraps the student's selective discipline list grouped by catalog year.
    /// </summary>
    public class StudentOwnSelectiveDisciplinesDto
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string ThirdName { get; set; } = string.Empty;
        public string StudentName => $"{SecondName} {FirstName} {ThirdName}".Trim();
        public List<StudentOwnSelectiveDisciplineItemDto> SelectiveDisciplines { get; set; } = new();
    }

    // ── Educational program (main disciplines) with student grades ────────────

    /// <summary>
    /// A main discipline entry enriched with the student's grade.
    /// </summary>
    public class MainDisciplineWithGradeDto
    {
        public Guid IdMainDisciplines { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Semestr { get; set; }
        public double? Loans { get; set; }
        public int? Hours { get; set; }
        public string? TypeOfControlName { get; set; }
        /// <summary>Null when the student has no grade yet.</summary>
        public int? Grade { get; set; }
        public bool? IsRedo { get; set; }
    }

    /// <summary>
    /// Educational program main-discipline list grouped by semester, with student grades.
    /// </summary>
    public class StudentEducationalProgramWithGradesDto
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string ThirdName { get; set; } = string.Empty;
        public string StudentName => $"{SecondName} {FirstName} {ThirdName}".Trim();
        public Guid EducationalProgramId { get; set; }
        public string EducationalProgramName { get; set; } = null!;
        /// <summary>Semester number → disciplines in that semester.</summary>
        public Dictionary<int, List<MainDisciplineWithGradeDto>> DisciplinesBySemester { get; set; } = new();
    }
}
