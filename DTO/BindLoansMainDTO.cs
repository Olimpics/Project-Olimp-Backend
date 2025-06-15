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

    public class BindLoansMainWithDetailsDto
    {
        public int IdBindLoan { get; set; }
        public int AddDisciplinesId { get; set; }
        public int EducationalProgramId { get; set; }
        public string AddDisciplineName { get; set; }
        public string EducationalProgramName { get; set; }
        public string Faculty { get; set; }
        public string Speciality { get; set; }
        public string DepartmentName { get; set; }
        public string? Teacher { get; set; }
        public string? Recomend { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Determination { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }
        public string? AdditionaLiterature { get; set; }
        public string TypesOfTraining { get; set; }
        public string TypeOfControll { get; set; }
    }

    public class CreateBindLoansMainWithDetailsDto
    {
        public int AddDisciplinesId { get; set; }
        public int EducationalProgramId { get; set; }
        public CreateAddDetailDto Details { get; set; }
    }

    public class UpdateBindLoansMainWithDetailsDto : CreateBindLoansMainWithDetailsDto
    {
        public int IdBindLoan { get; set; }
    }
} 