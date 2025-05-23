namespace OlimpBack.DTO
{
    public class BindMainDisciplineDto
    {
        public int IdBindMainDisciplines { get; set; }
        public string CodeMainDisciplines { get; set; } = null!;
        public string NameBindMainDisciplines { get; set; } = null!;
        public int Loans { get; set; }
        public string? FormControll { get; set; }
        public int Semestr { get; set; }
        public string EducationalProgramName { get; set; } = null!;
    }

    public class CreateBindMainDisciplineDto
    {
        public string CodeMainDisciplines { get; set; } = null!;
        public string NameBindMainDisciplines { get; set; } = null!;
        public int Loans { get; set; }
        public string? FormControll { get; set; }
        public int Semestr { get; set; }
        public int EducationalProgramId { get; set; }
    }

    public class UpdateBindMainDisciplineDto : CreateBindMainDisciplineDto
    {
        public int IdBindMainDisciplines { get; set; }
    }

}
