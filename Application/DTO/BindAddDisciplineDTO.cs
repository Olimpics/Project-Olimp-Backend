using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    public class BindAddDisciplineDto
    {
        public int IdBindAddDisciplines { get; set; }
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public int AddDisciplinesId { get; set; }
        public string AddDisciplineName { get; set; } = string.Empty;
        public int Semestr { get; set; }
        public int Loans { get; set; }
        public bool InProcess { get; set; }
    }

    public class CreateBindAddDisciplineDto
    {
        public int StudentId { get; set; }
        public int AddDisciplinesId { get; set; }
        public int Semestr { get; set; }
        public int Loans { get; set; }
    }

    public class UpdateBindAddDisciplineDto : CreateBindAddDisciplineDto
    {
        public int IdBindAddDisciplines { get; set; }
    }

}
