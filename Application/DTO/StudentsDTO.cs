using System;

namespace OlimpBack.Application.DTO
{
    // DTO/StudentDto.cs
    public class StudentDto
    {
        public Guid IdStudent { get; set; }
        public string NameStudent { get; set; } = null!;
        public string? StatusName { get; set; }
        public string? FacultyName { get; set; }
        public string? ProgramName { get; set; }
        public string? DegreeName { get; set; }
        public string? StudyFormName { get; set; }
        public string? GroupName { get; set; }
        public DateOnly? EducationStart { get; set; }
        public DateOnly? EducationEnd { get; set; }
        public int Course { get; set; }
        public bool IsShort { get; set; }
    }
    public class StudentForCatalogDto
    {
        public Guid IdStudent { get; set; }
        public string NameStudent { get; set; } = null!;
        public string? FacultyAbbreviation { get; set; }
        public string? SpecialityCode { get; set; }
        public string? Speciality { get; set; }
        public string? DegreeName { get; set; }
        public string? GroupName { get; set; }
        public int Course { get; set; }
        public bool IsShort { get; set; }
    }

    // DTO/CreateStudentDto.cs
    public class CreateStudentDto
    {
        public Guid IdStudent { get; set; }
        public Guid UserId { get; set; }
        public string NameStudent { get; set; } = null!;
        public Guid StatusId { get; set; }
        public DateOnly? EducationStart { get; set; }
        public DateOnly? EducationEnd { get; set; }
        public int Course { get; set; }
        public Guid FacultyId { get; set; }
        public Guid EducationalDegreeId { get; set; }
        public Guid StudyFormId { get; set; }
        public bool IsShort { get; set; }
        public Guid EducationalProgramId { get; set; }
        public Guid GroupId { get; set; }
        public Guid DepartmentId { get; set; }
        
    }

    // DTO/UpdateStudentDto.cs
    public class UpdateStudentDto : CreateStudentDto
    {
    }

}
