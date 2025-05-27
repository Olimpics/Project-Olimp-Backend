using AutoMapper;
using OlimpBack.Models;
using OlimpBack.DTO;

namespace OlimpBack.MappingProfiles
{
    
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User
            CreateMap<User, UserRoleDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.NameRole));
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.IdUsers, opt => opt.Ignore());

            CreateMap<User, UpdateUserDto>();


            //Student
            CreateMap<Student, StudentDto>()
    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.NameEducationStatus))
    .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.NameFaculty))
    .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram))
    .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.EducationalDegree.NameEducationalDegreec))
    .ForMember(dest => dest.StudyFormName, opt => opt.MapFrom(src => src.StudyForm.NameStudyForm));

            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();

            //AddDisciplines
            CreateMap<AddDiscipline, AddDisciplineDto>();
            CreateMap<CreateAddDisciplineDto, AddDiscipline>();
            CreateMap<AddDiscipline, SimpleDisciplineDto>();
            CreateMap<AddDiscipline, FullDisciplineDto>();

            // BindAddDiscipline
            CreateMap<BindAddDiscipline, BindAddDisciplineDto>()
                .ForMember(dest => dest.StudentFullName,
                           opt => opt.MapFrom(src => src.Student.NameStudent))
                .ForMember(dest => dest.AddDisciplineName,
                           opt => opt.MapFrom(src => src.AddDisciplines.NameAddDisciplines));

            CreateMap<CreateBindAddDisciplineDto, BindAddDiscipline>();
            CreateMap<UpdateBindAddDisciplineDto, BindAddDiscipline>();

            //BindMainDiscipline
            CreateMap<BindMainDiscipline, BindMainDisciplineDto>()
           .ForMember(dest => dest.EducationalProgramName,
                      opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram));

            CreateMap<CreateBindMainDisciplineDto, BindMainDiscipline>();
            CreateMap<UpdateBindMainDisciplineDto, BindMainDiscipline>();

            //EducationalDegree
            CreateMap<EducationalDegree, EducationalDegreeDto>()
            .ForMember(dest => dest.StudentsCount,
                       opt => opt.MapFrom(src => src.Students.Count));

            CreateMap<CreateEducationalDegreeDto, EducationalDegree>();
            CreateMap<UpdateEducationalDegreeDto, EducationalDegree>();

            //EducationalProgram
            CreateMap<EducationalProgram, EducationalProgramDto>()
          .ForMember(dest => dest.StudentsCount,
                     opt => opt.MapFrom(src => src.Students.Count))
          .ForMember(dest => dest.DisciplinesCount,
                     opt => opt.MapFrom(src => src.BindMainDisciplines.Count));

            CreateMap<CreateEducationalProgramDto, EducationalProgram>();
            CreateMap<UpdateEducationalProgramDto, EducationalProgram>();

            //EducationStatus
            CreateMap<EducationStatus, EducationStatusDto>().ReverseMap();

            //Faculty
            CreateMap<Faculty, FacultyDto>().ReverseMap();

            //Role
            CreateMap<Role, RoleDto>().ReverseMap();

            //StudyForm
            CreateMap<StudyForm, StudyFormDto>().ReverseMap();

            //DisciplineTab
            CreateMap<(Student student, List<AddDiscipline> disciplines, int currentCourse, bool isEvenSemester), DisciplineTabResponseDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.student.IdStudents))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.student.NameStudent))
                .ForMember(dest => dest.CurrentCourse, opt => opt.MapFrom(src => src.currentCourse))
                .ForMember(dest => dest.IsEvenSemester, opt => opt.MapFrom(src => src.isEvenSemester))
                .ForMember(dest => dest.Disciplines, opt => opt.MapFrom(src => src.disciplines));
        }

    }

}
