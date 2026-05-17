using System;
using System.Collections.Generic;
using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    /// <summary>
    /// One selected discipline in admin list (with bind id and confirmation state).
    /// </summary>
    public class StudentSelectedDisciplineDto
    {
        public Guid IdBindSelectiveDisciplines { get; set; }
        public Guid IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public int Semestr { get; set; }
        /// <summary>true = in process (awaiting confirmation), false = confirmed.</summary>
        public bool InProcess { get; set; }
    }

    /// <summary>
    /// Student with selected disciplines for admin discipline tab.
    /// </summary>
    public class StudentWithDisciplineChoicesDto
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? SecondName { get; set; }
        public string? ThirdName { get; set; }
        public string FullName => $"{SecondName} {FirstName} {ThirdName}".Trim();
        public string Faculty { get; set; } = null!;
        public string Group { get; set; } = null!;
        public int Year { get; set; }
        public Guid DegreeLevelId { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        public List<StudentSelectedDisciplineDto> SelectedDisciplines { get; set; } = new();
        /// <summary>false = not all required for the semester selected, true = all selected.</summary>
        public bool SelectionStatus { get; set; }
        /// <summary>false = not all confirmed, true = all confirmed.</summary>
        public bool ConfirmationStatus { get; set; }
    }

    /// <summary>
    /// Request to confirm or reject a student's elective choice.
    /// </summary>
    public class ConfirmOrRejectChoiceDto
    {
        public Guid BindId { get; set; }
        /// <summary>true = Confirm, false = Reject.</summary>
        public bool IsConfirm { get; set; }
    }


    /// <summary>
    /// One discipline in admin list with availability and normative status.
    /// </summary>
    public class AdminDisciplineListItemDto
    {
        public Guid IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string? Teachers { get; set; }
        public string? DepartmentName { get; set; }
        public int? Credits { get; set; }
        /// <summary>Normative count from Normative table by DegreeLevelId and IsFaculty.</summary>
        public int? Normative { get; set; }
        public int? MaxCountPeople { get; set; }
        public int CurrentCount { get; set; }
        /// <summary>Accepted | Smartly Acquired | Not Acquired (or overridden by admin).</summary>
        public string Status { get; set; } = null!;
        public bool IsForceChange { get; set; }
        public Guid? DegreeLevelId { get; set; }
        public bool IsFaculty { get; set; }
        public Guid FacultyId { get; set; }
        public string? FacultyAbbreviation { get; set; }
    }

    /// <summary>
    /// Request to set discipline status (admin override).
    /// Status (DB ids): 1 = Not Selected, 2 = Intellectually Selected, 3 = Selected, 4 = Collected.
    /// </summary>
    public class UpdateDisciplineStatusDto
    {
        public Guid DisciplineId { get; set; }
        /// <summary>1 = Not Selected, 2 = Intellectually Selected, 3 = Selected, 4 = Collected.</summary>
        public int Status { get; set; }
    }

    /// <summary>
    /// Query parameters for GetStudentsWithDisciplineChoices.
    /// </summary>
    /// 

    public class GetStudentsWithDisciplineChoicesQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? Search { get; set; }

        public List<Guid>? Faculties { get; set; }
        public List<int>? Courses { get; set; }
        public List<Guid>? StudentGroups { get; set; }
        public List<Guid>? DegreeLevelIds { get; set; }

        public bool? SelectionStatus { get; set; }
        public bool? ConfirmationStatus { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsNew { get; set; } = true;
        public Guid FacultyId { get; set; } = Guid.Empty;
    }

    /// <summary>
    /// For mapping.
    /// </summary>
    public class FullForAdminDisciplineDto
    {
        public Guid IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public List<int>? Courses { get; set; }
        public bool? IsEven { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        public int CountOfPeople { get; set; }
        public Guid? CatalogId { get; set; }
        public Guid? ApprovalStatusId { get; set; }
        public Guid? TypeOfControlId { get; set; }
    }

    public class GetAllDisciplinesAdminQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Search { get; set; }
        public Guid? CatalogId { get; set; }
        public List<Guid>? Faculties { get; set; }
        public List<Guid>? Departments { get; set; }
        public List<int>? Courses { get; set; }
        public bool? IsEvenSemester { get; set; }
        public List<Guid>? DegreeLevelIds { get; set; }
        public List<Guid>? TypeOfControlIds { get; set; }
        public List<Guid>? ApprovalStatusIds { get; set; }
        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// Query parameters for GetDisciplinesWithStatus.
    /// </summary>
    public class GetDisciplinesWithStatusQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? Search { get; set; }

        public List<Guid>? Faculties { get; set; }
        public bool? IsFaculty { get; set; }
        public List<Guid>? DegreeLevelIds { get; set; }
        public List<Guid>? TypeOfControlIds { get; set; }
        public List<Guid>? ApprovalStatusIds { get; set; }

        public int? StatusFilter { get; set; }
        public int SortOrder { get; set; } = 0;
    }


    // DTO   / 
    public class UpdateChoiceResponseDto
    {
        public List<ChoiceResultDto> Results { get; set; } = new();
        public List<ChoiceErrorDto> Errors { get; set; } = new();
    }

    public class ChoiceResultDto
    {
        public string Message { get; set; } = null!;
        public Guid BindId { get; set; }
        public string? DisciplineName { get; set; }
        public Guid? NotificationId { get; set; } // Nullable, 
    }

    public class ChoiceErrorDto
    {
        public Guid BindId { get; set; }
        public string Error { get; set; } = null!;
    }

    public class UpdateDisciplineStatusResponseDto
    {
        public string Message { get; set; } = null!;
        public Guid DisciplineId { get; set; }
        public string Status { get; set; } = null!;
        public bool IsForceChange { get; set; }
    }

    // 
    public record StudentChoicesProjection(Guid IdStudent, 
        string FirstName, 
        string? SecondName,
        string? ThirdName,
        string FacultyName, 
        string GroupCode, 
        int Course, 
        Guid EducationalDegreeId, 
        string DegreeName, 
        EducationalProgram? Program, 
        List<StudentSelectedDisciplineDto> SelectedDisciplines);

    public record DisciplineStatusProjection(Guid IdSelectiveDisciplines, 
        string NameSelectiveDisciplines, 
        string? Teachers, 
        string? DepartmentName, 
        int? MinCountPeople,
        int? MaxCountPeople,
        bool IsForseChange,
        string TypeName, 
        Guid? DegreeLevelId, 
        bool IsFaculty, 
        Guid FacultyId, 
        string? FacultyAbbreviation, 
        List<string?> BindCreatedAtRaw);
}
