using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class GroupDto
    {
        public Guid IdGroup { get; set; }
        public string GroupCode { get; set; } = null!;
        public int? NumberOfStudents { get; set; }
    }

    public class GroupMainDisciplineDto
    {   
        //public Guid idGroup { get; set; }
        public Guid idMainDisciplines { get; set; }
        public string nameMainDisciplines { get; set; } = null!;
        public int? Semestr { get; set; }
        public int? Loans { get; set; }
        public int? Hours { get; set; }
    }

    public class GroupCurriculumDTO
    {
        public Guid IdGroup { get; set; }
        public string GroupCode { get; set; } = null!;
        public List<GroupMainDisciplineDto> MainDisciplines { get; set; } = new List<GroupMainDisciplineDto>();
    }


    public class CreateGroupDto
    {
        public string GroupCode { get; set; } = null!;
        public int? NumberOfStudents { get; set; }
    }

    public class UpdateGroupDto : CreateGroupDto
    {
        public Guid IdGroup { get; set; }
    }

    public class GroupDetailsDto
    {
        public Guid IdGroup { get; set; }
        public string GroupCode { get; set; } = null!;
        public int? NumberOfStudents { get; set; }
        public Guid? AdminId { get; set; }
        public Guid? DegreeLevelId { get; set; }
        public int? Course { get; set; }
        public Guid? FacultyId { get; set; }
        public string? FacultyName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? IdEducationalProgram { get; set; }
        public string? EducationalProgramName { get; set; }
        public Guid? IdSpeciality { get; set; }
        public string? SpecialityName { get; set; }
        public int? AdmissionYear { get; set; }
        public Guid? IdStudyForm { get; set; }
        public Guid? IdSpecialization { get; set; }
        public string? SpecializationName { get; set; }
        public bool IsAccelerated { get; set; }
    }

    public class GroupStudentDto
    {
        public Guid IdStudent { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? SecondName { get; set; }
        public string? ThirdName { get; set; }
        public string EmailStudent { get; set; } = null!;
        public string EductionalStatus { get; set; } = null!;
    }
} 
 