using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

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

            // Login mapping
            CreateMap<Student, LoginResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdStudent))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.User.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameStudent))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Faculty.NameFaculty))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.EducationalProgram.Speciality))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.DegreeLevel, opt => opt.MapFrom(src => src.EducationalDegree.NameEducationalDegreec));

            CreateMap<Student, LoginResponseWithTokenDto>()
                .IncludeBase<Student, LoginResponseDto>();

            CreateMap<AdminsPersonal, LoginResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdAdmins))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.User.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameAdmin))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Faculty.NameFaculty));

            CreateMap<AdminsPersonal, LoginResponseWithTokenDto>()
                .IncludeBase<AdminsPersonal, LoginResponseDto>();

            //Student
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.NameEducationStatus))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.NameFaculty))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.EducationalDegree.NameEducationalDegreec))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupCode))
                .ForMember(dest => dest.StudyFormName, opt => opt.MapFrom(src => src.StudyForm.NameStudyForm));

            CreateMap<Student, StudentForCatalogDto>()
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.Faculty.Abbreviation)) 
                .ForMember(dest => dest.SpecialityCode, opt => opt.MapFrom(src => src.EducationalProgram.SpecialityCode))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.EducationalProgram.Speciality))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.EducationalDegree.NameEducationalDegreec))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupCode));

            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();

            //AddDisciplines
            CreateMap<AddDiscipline, AddDisciplineDto>()
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegreec));

            CreateMap<AddDiscipline, FullDisciplineDto>()
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegreec))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.Faculty.Abbreviation));

            CreateMap<AddDiscipline, FullForAdminDisciplineDto>()
              .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegreec));
            CreateMap<CreateAddDisciplineDto, AddDiscipline>()
                .ForMember(dest => dest.DegreeLevelId, opt => opt.MapFrom(src => src.DegreeLevelId));
            CreateMap<AddDiscipline, SimpleDisciplineDto>();

            CreateMap<CreateAddDisciplineWithDetailsDto, AddDiscipline>()
                .IncludeBase<CreateAddDisciplineDto, AddDiscipline>();

            CreateMap<UpdateAddDisciplineWithDetailsDto, AddDiscipline>()
                .IncludeBase<CreateAddDisciplineDto, AddDiscipline>();

            // BindAddDiscipline
            CreateMap<BindAddDiscipline, BindAddDisciplineDto>()
                .ForMember(dest => dest.StudentFullName,
                           opt => opt.MapFrom(src => src.Student.NameStudent))
                .ForMember(dest => dest.AddDisciplineName,
                           opt => opt.MapFrom(src => src.AddDisciplines.NameAddDisciplines))
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(src => src.InProcess == (sbyte)1));

            CreateMap<CreateBindAddDisciplineDto, BindAddDiscipline>()
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(src => (sbyte)1));

            CreateMap<UpdateBindAddDisciplineDto, BindAddDiscipline>()
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(src => (sbyte)1));

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
          .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree.NameEducationalDegreec))
          .ForMember(dest => dest.DisciplinesCount,
                     opt => opt.MapFrom(src => src.BindMainDisciplines.Count));

            CreateMap<CreateEducationalProgramDto, EducationalProgram>();
            CreateMap<UpdateEducationalProgramDto, EducationalProgram>();

            //EducationStatus
            CreateMap<EducationStatus, EducationStatusDto>().ReverseMap();

            //FacultyAbbreviation
            CreateMap<Faculty, FacultyDto>().ReverseMap();
            CreateMap<FacultyCreateDto, Faculty>();


            //Filters
            CreateMap<Department, FiltersDepartmentDTO>();

            //Role
            CreateMap<Role, RoleDto>().ReverseMap();

            //StudyForm
            CreateMap<StudyForm, StudyFormDto>().ReverseMap();

            //Group
            CreateMap<Group, GroupDto>().ReverseMap();
            CreateMap<CreateGroupDto, Group>();
            CreateMap<UpdateGroupDto, Group>();

            // GroupFilterDto mapping
            CreateMap<Group, GroupFilterDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdGroup))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.GroupCode))
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Students.Count))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.NameFaculty))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.NameDepartment))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.DegreeId, opt => opt.MapFrom(src => src.DegreeId))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.Degree.NameEducationalDegreec));

            //DisciplineTab
            CreateMap<(Student student, List<AddDiscipline> disciplines, int currentCourse, bool isEvenSemester), DisciplineTabResponseDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.student.IdStudent))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.student.NameStudent))
                .ForMember(dest => dest.CurrentCourse, opt => opt.MapFrom(src => src.currentCourse))
                .ForMember(dest => dest.IsEvenSemester, opt => opt.MapFrom(src => src.isEvenSemester))
                .ForMember(dest => dest.Disciplines, opt => opt.MapFrom(src => src.disciplines));

            CreateMap<(AddDiscipline discipline, AddDetail details), FullDisciplineWithDetailsDto>()
                .ForMember(dest => dest.IdAddDisciplines, opt => opt.MapFrom(src => src.discipline.IdAddDisciplines))
                .ForMember(dest => dest.NameAddDisciplines, opt => opt.MapFrom(src => src.discipline.NameAddDisciplines))
                .ForMember(dest => dest.CodeAddDisciplines, opt => opt.MapFrom(src => src.discipline.CodeAddDisciplines))
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.discipline.Faculty.Abbreviation))
                .ForMember(dest => dest.MinCountPeople, opt => opt.MapFrom(src => src.discipline.MinCountPeople))
                .ForMember(dest => dest.MaxCountPeople, opt => opt.MapFrom(src => src.discipline.MaxCountPeople))
                .ForMember(dest => dest.MinCourse, opt => opt.MapFrom(src => src.discipline.MinCourse))
                .ForMember(dest => dest.MaxCourse, opt => opt.MapFrom(src => src.discipline.MaxCourse))
                .ForMember(dest => dest.AddSemestr, opt => opt.MapFrom(src => src.discipline.AddSemestr))
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.discipline.DegreeLevel.NameEducationalDegreec))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.details.Department.NameDepartment))
                .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.details.Teachers))
                .ForMember(dest => dest.Recomend, opt => opt.MapFrom(src => src.details.Recomend))
                .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src => src.details.Prerequisites))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.details.Language))
                .ForMember(dest => dest.Determination, opt => opt.MapFrom(src => src.details.Determination))
                .ForMember(dest => dest.WhyInterestingDetermination, opt => opt.MapFrom(src => src.details.WhyInterestingDetermination))
                .ForMember(dest => dest.ResultEducation, opt => opt.MapFrom(src => src.details.ResultEducation))
                .ForMember(dest => dest.UsingIrl, opt => opt.MapFrom(src => src.details.UsingIrl))
                .ForMember(dest => dest.AdditionaLiterature, opt => opt.MapFrom(src => src.details.AdditionaLiterature))
                .ForMember(dest => dest.TypesOfTraining, opt => opt.MapFrom(src => src.details.TypesOfTraining))
                .ForMember(dest => dest.TypeOfControll, opt => opt.MapFrom(src => src.details.TypeOfControll));

            //Department
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.NameFaculty));
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            //BindLoansMain
            CreateMap<BindLoansMain, BindLoansMainDto>()
                .ForMember(dest => dest.AddDisciplineName, opt => opt.MapFrom(src => src.AddDisciplines.NameAddDisciplines))
                .ForMember(dest => dest.CodeAddDisciplines, opt => opt.MapFrom(src => src.AddDisciplines.CodeAddDisciplines))
                .ForMember(dest => dest.SpecialityCode, opt => opt.MapFrom(src => src.EducationalProgram.SpecialityCode))
                .ForMember(dest => dest.EducationalProgramName, opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram));
            CreateMap<CreateBindLoansMainDto, BindLoansMain>();
            CreateMap<UpdateBindLoansMainDto, BindLoansMain>();

            //AddDetail
            CreateMap<AddDetail, AddDetailDto>()
               .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.NameDepartment));
            CreateMap<CreateAddDetailDto, AddDetail>()
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId));
            CreateMap<UpdateAddDetailDto, AddDetail>();

            //Notification
             CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead));
        
            CreateMap<CreateNotificationDto, Notification>();
            CreateMap<UpdateNotificationDto, Notification>();

            //NotificationTemplate
            CreateMap<NotificationTemplate, NotificationTemplateDto>();
            CreateMap<CreateNotificationTemplateDto, NotificationTemplate>();
            CreateMap<UpdateNotificationTemplateDto, NotificationTemplate>();

            //Permission
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<CreatePermissionDto, Permission>();
            CreateMap<UpdatePermissionDto, Permission>();

            //BindRolePermission
            CreateMap<BindRolePermission, BindRolePermissionDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.NameRole))
                .ForMember(dest => dest.TypePermission, opt => opt.MapFrom(src => src.Permission.TypePermission))
                .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Permission.TableName));
            CreateMap<CreateBindRolePermissionDto, BindRolePermission>();
            CreateMap<UpdateBindRolePermissionDto, BindRolePermission>();

            //DisciplineChoicePeriod
            CreateMap<DisciplineChoicePeriod, DisciplineChoicePeriodDto>()
    .ForMember(
        dest => dest.Id,
        opt => opt.MapFrom(src => src.IdDisciplineChoicePeriod)
    )
    .ReverseMap()
    .ForMember(
        dest => dest.IdDisciplineChoicePeriod,
        opt => opt.MapFrom(src => src.Id)
    );
            CreateMap<CreateDisciplineChoicePeriodDto, DisciplineChoicePeriod>()
                .ForMember(dest => dest.DegreeLevelId, opt => opt.MapFrom(src => src.DegreeLevelId));
            CreateMap<UpdateDisciplineChoicePeriodDto, DisciplineChoicePeriod>()
                .ForMember(dest => dest.DegreeLevelId, opt => opt.MapFrom(src => src.DegreeLevelId));
            CreateMap<UpdateDisciplineChoicePeriodAfterStartDto, DisciplineChoicePeriod>();
            CreateMap<UpdateDisciplineChoicePeriodOpenOrCloseDto, DisciplineChoicePeriod>();

        }

    }

}
