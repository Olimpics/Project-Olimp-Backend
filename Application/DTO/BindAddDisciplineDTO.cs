using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    public class BindSelectiveDisciplineDto
    {
        public int IdBindSelectiveDisciplines { get; set; }
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public int SelectiveDisciplinesId { get; set; }
        public string SelectiveDisciplineName { get; set; } = string.Empty;
        public int Semestr { get; set; }
        public int Loans { get; set; }
        public bool InProcess { get; set; }
    }

    public class CreateBindSelectiveDisciplineDto
    {
        public int StudentId { get; set; }
        public int SelectiveDisciplinesId { get; set; }
        public int Semestr { get; set; }
        public int Loans { get; set; }
    }

    public class UpdateBindSelectiveDisciplineDto : CreateBindSelectiveDisciplineDto
    {
        public int IdBindSelectiveDisciplines { get; set; }
    }

}
