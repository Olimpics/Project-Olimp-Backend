using System;
using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    public class BindSelectiveDisciplineDto
    {
        public Guid IdBindSelectiveDisciplines { get; set; }
        public Guid StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public Guid SelectiveDisciplinesId { get; set; }
        public string SelectiveDisciplineName { get; set; } = string.Empty;
        public int Semestr { get; set; }
        public int Loans { get; set; }
        public bool InProcess { get; set; }
    }

    public class CreateBindSelectiveDisciplineDto
    {
        public Guid StudentId { get; set; }
        public Guid SelectiveDisciplinesId { get; set; }
        public int Semestr { get; set; }
        public int Loans { get; set; }
    }

    public class UpdateBindSelectiveDisciplineDto : CreateBindSelectiveDisciplineDto
    {
        public Guid IdBindSelectiveDisciplines { get; set; }
    }

}
