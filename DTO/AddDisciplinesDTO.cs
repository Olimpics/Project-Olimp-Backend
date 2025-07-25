﻿using OlimpBack.Models;

namespace OlimpBack.DTO
{
    public class AddDisciplineDto
    {
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
        public int FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public int? AddSemestr { get; set; }
        public string DegreeLevelId { get; set; }
        public string DegreeLevelName { get; set; }
    }

    public class CreateAddDisciplineDto
    {
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
        public int FacultyId { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public int? AddSemestr { get; set; }
        public int? DegreeLevelId { get; set; }
    }

    public class CreateAddDisciplineWithDetailsDto : CreateAddDisciplineDto
    {
        public CreateAddDetailDto Details { get; set; } = null!;
    }

    public class UpdateAddDisciplineWithDetailsDto : CreateAddDisciplineWithDetailsDto
    {
        public int IdAddDisciplines { get; set; }
    }
}
