namespace OlimpBack.DTO
{
    public class BindLoansMainDto
    {
        public int IdBindLoan { get; set; }
        public int AddDisciplinesId { get; set; }
        public string AddDisciplineName { get; set; } = null!;
        public int EducationalProgramId { get; set; }
        public string EducationalProgramName { get; set; } = null!;
    }

    public class CreateBindLoansMainDto
    {
        public int AddDisciplinesId { get; set; }
        public int EducationalProgramId { get; set; }
    }

    public class UpdateBindLoansMainDto : CreateBindLoansMainDto
    {
        public int IdBindLoan { get; set; }
    }
} 