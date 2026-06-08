using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class EducationalProgramDto
    {
        public Guid IdEducationalProgram { get; set; }
        public string NameEducationalProgram { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public string Speciality { get; set; } = null!;
        public string StudyForm { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public int? StudyTurm { get; set; }
        public string IsAccelerated { get; set; } = null!;
    }

    public class CatalogDto
    {
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }

    public class EducationalProgramListQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Search { get; set; }

        //   !
        public List<Guid>? DegreeLevelIds { get; set; }

        public int SortOrder { get; set; } = 0;
    }
    public class EducationalProgramFullDto : EducationalProgramDto
    {
        public List<int> SelectiveDisciplineBySemestr { get; set; } = new();
        public List<int> MinUniSelectiveDisciplineBySemestr { get; set; } = new();
        public string Subject { get; set; } = null!;
        public string Goals { get; set; } = null!;
        public List<string>? Keys { get; set; }
        public CatalogDto Catalog { get; set; } = null!;
        public string DegreeLevelName { get; set; } = null!;
        public string? SpecializationName { get; set; }
        public string SpecialityName { get; set; } = null!;
        public string StudyFormName { get; set; } = null!;

        public Guid DegreeId { get; set; }
        public sbyte Accreditation { get; set; }
        public string AccreditationType { get; set; } = null!;

    }
    public class CreateEducationalProgramDto
    {
        public string NameEducationalProgram { get; set; } = null!;
        public List<int> SelectiveDisciplineBySemestr { get; set; } = new();

        //  string Degree   ID
        public Guid DegreeId { get; set; }

        public Guid SpecialityId { get; set; }
        public sbyte Accreditation { get; set; }
        public string AccreditationType { get; set; } = null!;
        public uint StudentsAmount { get; set; }
    }

    public class UpdateEducationalProgramDto : CreateEducationalProgramDto
    {
        public Guid IdEducationalProgram { get; set; }
    }

}
