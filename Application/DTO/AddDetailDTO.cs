namespace OlimpBack.Application.DTO
{

    // Це наша композитна частина (лише текстовий контент)
    public class DetailContentDto
    {
        public string? Teacher { get; set; }

        public string? Recomend { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Provision { get; set; }
        public string? DisciplineTopics { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }

        public string TypesOfTraining { get; set; } = null!;
        public string TypeOfControll { get; set; } = null!;
    }

    // DTO для читання (GET)
    public class AddDetailDto : AddDisciplineDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;

        // Композиція: цей DTO "має" контент
        public DetailContentDto Content { get; set; } = new();
    }

    // DTO для створення (POST/PUT)
    public class CreateAddDetailDto
    {
        public int? DepartmentId { get; set; }

        // Композиція: цей DTO теж "має" контент
        public DetailContentDto Content { get; set; } = new();
    }

} 