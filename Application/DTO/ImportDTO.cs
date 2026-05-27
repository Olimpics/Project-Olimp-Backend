using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OlimpBack.Application.DTO
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        public string TableName { get; set; } = null!;

        [Required]
        public bool IsCreate { get; set; }

        [Required]
        public int? Limit { get; set; }
    }

    public class ExportRequestDto
    {
        [Required]
        public string TableName { get; set; } = null!;
        public JsonElement? Request { get; set; } // Optional JSON request for filtering or options
    }

    public class SelectiveDisciplineWordContentDto
    {
        public string? CodeAndName { get; set; }
        public string? RecommendedForFields { get; set; }
        public string? Department { get; set; }
        public string? Instructor { get; set; }
        public string? EducationLevel { get; set; }
        public string? CourseAndSemester { get; set; }
        public string? LanguageOfInstruction { get; set; }
        public string? Prerequisites { get; set; }
        public string? WhyStudyThisCourse { get; set; }
        public string? TopicList { get; set; }
        public string? CompetenciesGained { get; set; }
        public string? ExpectedLearningOutcomes { get; set; }
        public string? InformationResources { get; set; }
        public string? TypesOfLearningActivities { get; set; }
        public string? SemesterControlType { get; set; }
        public string? MaxMinStudents { get; set; }
    }

    public class GeminiSelectiveDisciplineDto
    {
        public string? CodeSelectiveDisciplines { get; set; }
        public string? NameSelectiveDisciplines { get; set; }
        public string? NameSelectiveDisciplinesEng { get; set; }
        public RecommendedDto? Recommended { get; set; }
        public bool NeedFix { get; set; }
        public string? Department { get; set; }
        public List<string>? Teachers { get; set; }
        public Guid DegreeLevelId { get; set; }
        public List<int>? Courses { get; set; }
        public bool IsEven { get; set; }
        public string? Language { get; set; }
        public string? Prerequisites { get; set; }
        public string? WhyInterestingDetermination { get; set; }
        public string? Provision { get; set; }
        public string? UsingIrl { get; set; }
        public string? ResultEducation { get; set; }
        public List<string>? DisciplineTopics { get; set; }
        public string? TypesOfTraining { get; set; }
        public int? MinCountPeople { get; set; }
        public int? MaxCountPeople { get; set; }
    }

    public class RecommendedDto
    {
        public List<string>? Branches { get; set; }
        public List<string>? Specialities { get; set; }
        public List<string>? EducationalPrograms { get; set; }
    }

    public class SelectiveDisciplineImportRequestDto
    {
        [Required]
        public IFormFile Archive { get; set; } = null!;
        [Required]
        public Guid CatalogId { get; set; }
        [Required]
        public bool IsFaculty { get; set; }
    }

    public class EducationalProgramImportRequestDto
    {
        [Required]
        public IFormFile WordFile { get; set; } = null!;
        [Required]
        public IFormFile PdfFile { get; set; } = null!;
        [Required]
        public Guid CatalogYearMainId { get; set; }
        [Required]
        public bool IsAccelerated { get; set; }
        [Required]
        public int StudyTurm { get; set; }
    }

    public class EducationalProgramWordContentDto
    {
        public string? NameEducationalProgram { get; set; }
        public string? Degree { get; set; }
        public string? StudyForm { get; set; }
        public string? Goals { get; set; }
        public string? SpecialityAndSpecializationWithDetails { get; set; }
        public string? Subject { get; set; }
        public List<DisciplineRowDto> MainDisciplines { get; set; } = new();
        public List<string> MainDisciplinesNeedFix { get; set; } = new();
        public List<DisciplineRowDto> SelectiveDisciplines { get; set; } = new();
    }

    public class DisciplineRowDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Loans { get; set; }
        public string? Control { get; set; }
        public string? Semester { get; set; }
    }

    public class GeminiEducationalProgramDto
    {
        public string? NameEducationalProgram { get; set; }
        public string? Degree { get; set; }
        public string? StudyForm { get; set; }
        public string? Subject { get; set; }
        public string? Speciality { get; set; }
        public string? Specialization { get; set; }
        public string? Goals { get; set; }
        public List<GeminiMainDisciplineDto> MainDisciplines { get; set; } = new();
        public List<GeminiMainDisciplineDto> SelectiveDisciplines { get; set; } = new();
        public List<int> SelectiveDisciplineBySemestr { get; set; } = new();
    }

    public class GeminiMainDisciplineDto
    {
        public string Code { get; set; }
        public string? Name { get; set; }
        public int? Loans { get; set; }
        public string? Control { get; set; }
        public int? Semester { get; set; }
    }
}
