namespace OlimpBack.Application.DTO
{
    public class MainDisciplineDto
    {
        public int IdMainDisciplines { get; set; }
        public string CodeMainDisciplines { get; set; } = null!;
        public string NameMainDisciplines { get; set; } = null!;
        public int Loans { get; set; }
        public string? FormControll { get; set; }
        public int Semestr { get; set; }
        public string Teachers { get; set; } = null!;
        public string EducationalProgramName { get; set; } = null!;
    }

    public class CreateMainDisciplineDto
    {
        public string CodeMainDisciplines { get; set; } = null!;
        public string NameMainDisciplines { get; set; } = null!;
        public int Loans { get; set; }
        public string? FormControll { get; set; }
        public int Semestr { get; set; }
        public int EducationalProgramId { get; set; }
        public string Teachers { get; set; } = null!;
    }

    public class UpdateMainDisciplineDto : CreateMainDisciplineDto
    {
        public int IdMainDisciplines { get; set; }
    }
}
