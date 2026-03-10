using System.Collections.Generic;

namespace OlimpBack.DTO
{
    public class BindLoansMainDto
    {
        public int IdBindLoan { get; set; }
        public int AddDisciplinesId { get; set; }
        public int EducationalProgramId { get; set; }
        public string CodeAddDisciplines { get; set; }
        public string AddDisciplineName { get; set; }
        public string SpecialityCode { get; set; }
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

    public class BindLoansMainQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Search { get; set; }
        public string? AddDisciplinesIds { get; set; }
        public string? EducationalProgramIds { get; set; }
        public string? Faculties { get; set; }
        public string? Specialities { get; set; }
        public int SortOrder { get; set; } = 0;
    }
} 