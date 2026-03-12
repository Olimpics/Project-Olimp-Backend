using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    /// <summary>
    /// One selected discipline in admin list (with bind id and confirmation state).
    /// </summary>
    public class StudentSelectedDisciplineDto
    {
        public int IdBindAddDisciplines { get; set; }
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
        public int Semestr { get; set; }
        /// <summary>1 = in process (awaiting confirmation), 0 = confirmed.</summary>
        public sbyte InProcess { get; set; }
    }

    /// <summary>
    /// Student with selected disciplines for admin discipline tab.
    /// </summary>
    public class StudentWithDisciplineChoicesDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public string Group { get; set; } = null!;
        public int Year { get; set; }
        public int DegreeLevelId { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        public List<StudentSelectedDisciplineDto> SelectedDisciplines { get; set; } = new();
        /// <summary>0 = not all required for the semester selected, 1 = all selected.</summary>
        public int SelectionStatus { get; set; }
        /// <summary>0 = not all confirmed, 1 = all confirmed.</summary>
        public int ConfirmationStatus { get; set; }
    }

    /// <summary>
    /// Request to confirm or reject a student's elective choice.
    /// </summary>
    public class ConfirmOrRejectChoiceDto
    {
        public int BindId { get; set; }
        /// <summary>"Confirm" or "Reject".</summary>
        public int IsConfirm { get; set; }
    }


    /// <summary>
    /// One discipline in admin list with availability and normative status.
    /// </summary>
    public class AdminDisciplineListItemDto
    {
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string? Teachers { get; set; }
        public string? DepartmentName { get; set; }
        public int? Credits { get; set; }
        /// <summary>Normative count from Normative table by DegreeLevelId and IsFaculty.</summary>
        public int? Normative { get; set; }
        public int? MaxCountPeople { get; set; }
        public int CurrentCount { get; set; }
        /// <summary>Accepted | Smartly Acquired | Not Acquired (or overridden by admin).</summary>
        public string Status { get; set; } = null!;
        public sbyte IsForceChange { get; set; }
        public int? DegreeLevelId { get; set; }
        public sbyte IsFaculty { get; set; }
        public int FacultyId { get; set; }
        public string? FacultyAbbreviation { get; set; }
    }

    /// <summary>
    /// Request to set discipline status (admin override).
    /// Status (DB ids): 1 = Not Selected, 2 = Intellectually Selected, 3 = Selected, 4 = Collected.
    /// </summary>
    public class UpdateDisciplineStatusDto
    {
        public int DisciplineId { get; set; }
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

        // Замість string? використовуємо List<int> (сказав джеміні)
        public List<int>? Faculties { get; set; }
        public List<int>? Courses { get; set; }
        public List<int>? Groups { get; set; }
        public List<int>? DegreeLevelIds { get; set; }

        public int? SelectionStatus { get; set; }
        public int? ConfirmationStatus { get; set; }
        public int SortOrder { get; set; } = 0;
        public int IsNew { get; set; } = 1;
        public int FacultyId { get; set; } = 0;
    }

    /// <summary>
    /// For mapping.
    /// </summary>
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

    /// <summary>
    /// Query parameters for GetDisciplinesWithStatus.
    /// </summary>
    public class GetDisciplinesWithStatusQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? Search { get; set; }

        // Замість string? використовуємо List<int>? (сказав джеміні)
        public List<int>? Faculties { get; set; }
        public sbyte? IsFaculty { get; set; }
        public List<int>? DegreeLevelIds { get; set; }

        public int? StatusFilter { get; set; }
        public int SortOrder { get; set; } = 0;
    }


    // Це допоміжні DTO для відповіді на підтвердження/відхилення вибору
    public class UpdateChoiceResponseDto
    {
        public List<ChoiceResultDto> Results { get; set; } = new();
        public List<ChoiceErrorDto> Errors { get; set; } = new();
    }

    public class ChoiceResultDto
    {
        public string Message { get; set; } = null!;
        public int BindId { get; set; }
        public string? DisciplineName { get; set; }
        public int? NotificationId { get; set; } // Nullable, бо при підтвердженні його немає
    }

    public class ChoiceErrorDto
    {
        public int BindId { get; set; }
        public string Error { get; set; } = null!;
    }

    public class UpdateDisciplineStatusResponseDto
    {
        public string Message { get; set; } = null!;
        public int DisciplineId { get; set; }
        public string Status { get; set; } = null!;
        public int IsForceChange { get; set; }
    }
}
