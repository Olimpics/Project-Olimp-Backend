using System.Collections.Generic;

namespace OlimpBack.DTO
{
    public class BindLoansMainDto
    {
        public int IdBindLoan { get; set; }
        public int AddDisciplinesId { get; set; }
        public int EducationalProgramId { get; set; }
        public string AddDisciplineName { get; set; }
        public string EducationalProgramName { get; set; }
       
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

    public class BindLoansMainFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Faculties { get; set; }
        public string? Specialities { get; set; }
    }
} 