using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    public class SelectiveDisciplineDto 
    {
        public int IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public int FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public sbyte? AddSemestr { get; set; } // make IsEven
        public string DegreeLevelId { get; set; }
        public string DegreeLevelName { get; set; }
    }

    public class CreateSelectiveDisciplineDto 
    {
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public int FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public int? IsEven { get; set; }
        public int? DegreeLevelId { get; set; }
    }

    public class CreateSelectiveDisciplineWithDetailsDto : CreateSelectiveDisciplineDto
    {
        public CreateSelectiveDetailDto Details { get; set; } = null!;

        public List<int>? RecomendationSpeciality { get; set; }
        public List<int>? RecomendationEducationalProgram { get; set; }
    }

    public class UpdateSelectiveDisciplineWithDetailsDto : CreateSelectiveDisciplineWithDetailsDto
    {
        public int IdSelectiveDisciplines { get; set; }
    }
}
