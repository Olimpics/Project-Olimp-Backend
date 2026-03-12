namespace OlimpBack.Application.DTO
{
    public class AddDetailDto : AddDisciplineDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string? Teacher { get; set; }
        public string? Recomend { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Determination { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }
        public string? AdditionaLiterature { get; set; }
        public string TypesOfTraining { get; set; } = null!;
        public string TypeOfControll { get; set; } = null!;
    }

    public class CreateAddDetailDto
    {
        public int? DepartmentId { get; set; }
        public string? Teacher { get; set; }
        public string? Recomend { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Determination { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }
        public string? AdditionaLiterature { get; set; }
        public string TypesOfTraining { get; set; } = null!;
        public string TypeOfControll { get; set; } = null!;
    }

} 