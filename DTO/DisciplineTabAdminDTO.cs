using System.Collections.Generic;

namespace OlimpBack.DTO
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
    /// Admin create bind request (studentId, disciplineId, semestr, optional loans).
    /// </summary>
    public class AdminCreateBindDto
    {
        public int StudentId { get; set; }
        public int DisciplineId { get; set; }
        public int Semestr { get; set; }
        public int Loans { get; set; } = 5;
    }
}
