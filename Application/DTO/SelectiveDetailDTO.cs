using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{

    // 
    public class DetailContentDto
    {
        public string? NameSelectiveDisciplinesEng { get; set; }
        public string? Teacher { get; set; }
        public string? Recomend { get; set; }
        public string? Prerequisites { get; set; }
        public string? Language { get; set; }
        public string? Provision { get; set; }
        public List<string>? DisciplineTopics { get; set; }
        public List<int>? ChangedTopicIndices { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? ResultEducation { get; set; }
        public string? UsingIrl { get; set; }
        public string TypesOfTraining { get; set; } = null!;
        public string TypeOfControl { get; set; } = null!;
    }

    // DTO  (GET)
    public class SelectiveDetailDto : SelectiveDisciplineDto
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;

        // :  DTO "" 
        public DetailContentDto Content { get; set; } = new();
    }

    // DTO  (POST/PUT)
    public class CreateSelectiveDetailDto
    {
        public Guid? DepartmentId { get; set; }

        // :  DTO  "" 
        public DetailContentDto Content { get; set; } = new();
    }

}
