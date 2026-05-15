using System;
using System.Collections.Generic;
using OlimpBack.Models;

namespace OlimpBack.Application.DTO
{
    public class SelectiveDisciplineDto 
    {
        public Guid IdSelectiveDisciplines { get; set; }
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public Guid FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public List<int>? Courses { get; set; }
        public bool? IsEven { get; set; }
        public Guid? DegreeLevelId { get; set; }
        public string DegreeLevelName { get; set; } = null!;
        public Guid? CatalogId { get; set; }
        public Guid? ApprovalStatusId { get; set; }
        public Guid? TypeOfControlId { get; set; }
    }

    public class CreateSelectiveDisciplineDto 
    {
        public string NameSelectiveDisciplines { get; set; } = null!;
        public string CodeSelectiveDisciplines { get; set; } = null!;
        public Guid FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public List<int>? Courses { get; set; }
        public bool? IsEven { get; set; }
        public Guid? DegreeLevelId { get; set; }
        public Guid? CatalogId { get; set; }
        public Guid? ApprovalStatusId { get; set; }
        public Guid? TypeOfControlId { get; set; }
    }

    public class CreateSelectiveDisciplineWithDetailsDto : CreateSelectiveDisciplineDto
    {
        public CreateSelectiveDetailDto Details { get; set; } = null!;

        public List<Guid>? AdminIds { get; set; }

        public List<int>? RecomendationCourses { get; set; }
        public List<Guid>? RecomendationSpeciality { get; set; }
        public List<Guid>? RecomendationEducationalProgram { get; set; }
    }

    public class UpdateSelectiveDisciplineWithDetailsDto : CreateSelectiveDisciplineWithDetailsDto
    {
        public Guid IdSelectiveDisciplines { get; set; }
    }
}
