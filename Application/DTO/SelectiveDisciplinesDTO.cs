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
        public List<int>? Courses { get; set; }
        public int? IsEven { get; set; }
        public int? DegreeLevelId { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        public int? CatalogId { get; set; }
        public int? ApprovalStatusId { get; set; }
        public int? TypeOfControlId { get; set; }
    }

    public class CreateSelectiveDisciplineDto 
    {
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public int FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public List<int>? Courses { get; set; }
        public int? IsEven { get; set; }
        public int? DegreeLevelId { get; set; }
        public int? CatalogId { get; set; }
        public int? ApprovalStatusId { get; set; }
        public int? TypeOfControlId { get; set; }
    }

    public class CreateSelectiveDisciplineWithDetailsDto : CreateSelectiveDisciplineDto
    {
        public CreateSelectiveDetailDto Details { get; set; } = null!;

        public List<int>? AdminIds { get; set; }

        public List<int>? RecomendationCourses { get; set; }
        public List<int>? RecomendationSpeciality { get; set; }
        public List<int>? RecomendationEducationalProgram { get; set; }
    }

    public class UpdateSelectiveDisciplineWithDetailsDto : CreateSelectiveDisciplineWithDetailsDto
    {
        public int IdSelectiveDisciplines { get; set; }
    }
}
