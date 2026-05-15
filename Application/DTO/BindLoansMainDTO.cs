using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class BindLoansMainDto
    {
        public Guid IdBindLoan { get; set; }
        public Guid SelectiveDisciplinesId { get; set; }
        public Guid EducationalProgramId { get; set; }
        public string? CodeSelectiveDisciplines { get; set; }
        public string? SelectiveDisciplineName { get; set; }
        public string? SpecialityCode { get; set; }
        public string? EducationalProgramName { get; set; }
       
    }

    public class CreateBindLoansMainDto
    {
        public Guid SelectiveDisciplinesId { get; set; }
        public Guid EducationalProgramId { get; set; }
    }

    public class UpdateBindLoansMainDto : CreateBindLoansMainDto
    {
        public Guid IdBindLoan { get; set; }
    }

    public class BindLoansMainQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Search { get; set; }

        public List<Guid>? SelectiveDisciplinesIds { get; set; }
        public List<Guid>? EducationalProgramIds { get; set; }

        public string? Faculties { get; set; }
        public string? Specialities { get; set; }
        public int SortOrder { get; set; } = 0;
    }
}
