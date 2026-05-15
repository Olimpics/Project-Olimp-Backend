using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;

namespace OlimpBack.MappingProfiles
{
    
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.IdUser, opt => opt.Ignore());

            CreateMap<User, UpdateUserDto>();

            // Login mapping
             CreateMap<Student, LoginResponseStudentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdStudent))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameStudent))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.FacultyId))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Name))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.DegreeLevel, opt => opt.MapFrom(src => src.Group.DegreeLevel.NameEducationalDegree));


            CreateMap<AdminsPersonal, LoginResponseAdminDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameAdmin))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Faculty.NameFaculty));


            //Student
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.EducationStatus.NameEducationStatus))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Group.EducationalProgram.NameEducationalProgram))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.Group.DegreeLevel.NameEducationalDegree))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupCode))
                .ForMember(dest => dest.StudyFormName, opt => opt.MapFrom(src => src.Group.StudyForm.NameStudyForm));

            CreateMap<Student, StudentForCatalogDto>()
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation)) 
                .ForMember(dest => dest.SpecialityCode, opt => opt.MapFrom(src => src.Group.EducationalProgram != null && src.Group.EducationalProgram.Speciality != null && src.Group.EducationalProgram.Speciality.Code.Length > 0 ? src.Group.EducationalProgram.Speciality.Code : ""))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Name))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.Group.DegreeLevel.NameEducationalDegree))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupCode));

            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();

            //SelectiveDisciplines
            CreateMap<SelectiveDiscipline, SelectiveDisciplineDto>()
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegree))
                .ForMember(dest => dest.IsEven, opt => opt.MapFrom(src => src.IsEven));

            CreateMap<SelectiveDiscipline, FullDisciplineDto>()
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegree))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.Department.FacultyId))
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.Department.Faculty.Abbreviation))
                .ForMember(dest => dest.IsEven, opt => opt.MapFrom(src => src.IsEven));

            CreateMap<SelectiveDiscipline, FullForAdminDisciplineDto>()
              .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegree));
            CreateMap<CreateSelectiveDisciplineDto, SelectiveDiscipline>()
                .ForMember(dest => dest.DegreeLevelId, opt => opt.MapFrom(src => src.DegreeLevelId))
                .ForMember(dest => dest.IsEven, opt => opt.MapFrom(src => src.IsEven));
            CreateMap<SelectiveDiscipline, SimpleDisciplineDto>();

            CreateMap<CreateSelectiveDisciplineWithDetailsDto, SelectiveDiscipline>()
                .IncludeBase<CreateSelectiveDisciplineDto, SelectiveDiscipline>();

            CreateMap<UpdateSelectiveDisciplineWithDetailsDto, SelectiveDiscipline>()
                .IncludeBase<CreateSelectiveDisciplineDto, SelectiveDiscipline>();


            // BindSelectiveDiscipline
            CreateMap<BindSelectiveDiscipline, BindSelectiveDisciplineDto>()
                .ForMember(dest => dest.StudentFullName,
                           opt => opt.MapFrom(src => src.Student.NameStudent))
                .ForMember(dest => dest.SelectiveDisciplineName,
                           opt => opt.MapFrom(src => src.SelectiveDiscipline.NameSelectiveDisciplines))
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(src => src.InProcess));

            CreateMap<CreateBindSelectiveDisciplineDto, BindSelectiveDiscipline>()
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(_ => true));

            CreateMap<UpdateBindSelectiveDisciplineDto, BindSelectiveDiscipline>()
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(_ => true));

            // Normative mapping
            CreateMap<Normative, NormativeDto>()
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel != null ? src.DegreeLevel.NameEducationalDegree : null));

            CreateMap<CreateNormativeDto, Normative>();
            CreateMap<UpdateNormativeDto, Normative>();


            //TypeOfDiscipline
            CreateMap<TypeOfDiscipline, TypeOfDisciplineDto>().ReverseMap();
            CreateMap<CreateTypeOfDisciplineDto, TypeOfDiscipline>();

            //MainDiscipline
            CreateMap<MainDiscipline, MainDisciplineDto>()
           .ForMember(dest => dest.EducationalProgramName,
                      opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram));

            CreateMap<CreateMainDisciplineDto, MainDiscipline>();
            CreateMap<UpdateMainDisciplineDto, MainDiscipline>();

            //EducationalDegree
            CreateMap<EducationalDegree, EducationalDegreeDto>()
            .ForMember(dest => dest.StudentsCount,
                       opt => opt.MapFrom(src => src.StudentGroups != null
                           ? src.StudentGroups.SelectMany(g => g.Students).Count()
                           : 0));

            CreateMap<CreateEducationalDegreeDto, EducationalDegree>();
            CreateMap<UpdateEducationalDegreeDto, EducationalDegree>();

            //EducationalProgram
            CreateMap<EducationalProgram, EducationalProgramDto>()
          .ForMember(dest => dest.StudentsCount,
                     opt => opt.MapFrom(src => src.StudentGroups != null
                         ? src.StudentGroups.SelectMany(g => g.Students).Count()
                         : 0))
          .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree.NameEducationalDegree))
          .ForMember(dest => dest.DisciplinesCount,
                     opt => opt.MapFrom(src => src.MainDisciplines.Count));

            CreateMap<CreateEducationalProgramDto, EducationalProgram>();
            CreateMap<UpdateEducationalProgramDto, EducationalProgram>();

            //EducationStatus
            CreateMap<EducationStatus, EducationStatusDto>().ReverseMap();

            //FacultyAbbreviation
            CreateMap<Faculty, FacultyDto>().ReverseMap();
            CreateMap<FacultyCreateDto, Faculty>();


            //CatalogYearMain
            CreateMap<CatalogYearsMain, CatalogYearMainDto>()
                .ForMember(dest => dest.IsFormed, opt => opt.MapFrom(src => src.IsFormed));
            CreateMap<CreateCatalogYearMainDto, CatalogYearsMain>();
            CreateMap<UpdateCatalogYearMainDto, CatalogYearsMain>();

            //CatalogYearSelective
            CreateMap<CatalogYearsSelective, CatalogYearSelectiveDto>()
                .ForMember(dest => dest.IsFormed, opt => opt.MapFrom(src => src.IsFormed));
            CreateMap<CreateCatalogYearSelectiveDto, CatalogYearsSelective>();
            CreateMap<UpdateCatalogYearSelectiveDto, CatalogYearsSelective>();


            //Filters
            CreateMap<Department, FiltersDepartmentDTO>();

            //Role
            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.IdRole, opt => opt.MapFrom(src => src.IdRole))
                .ForMember(dest => dest.NameRole, opt => opt.MapFrom(src => src.Name));
            CreateMap<RoleDto, Role>()
                .ForMember(dest => dest.IdRole, opt => opt.MapFrom(src => src.IdRole))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameRole))
                .ForMember(dest => dest.PermissionsMask, opt => opt.Ignore())
                .ForMember(dest => dest.ParentRoleId, opt => opt.Ignore());

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
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.Degree.NameEducationalDegree));

            //DisciplineTab
            CreateMap<(Student student, List<SelectiveDiscipline> disciplines, int currentCourse, bool isEvenSemester), DisciplineTabResponseDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.student.IdStudent))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.student.NameStudent))
                .ForMember(dest => dest.CurrentCourse, opt => opt.MapFrom(src => src.currentCourse))
                .ForMember(dest => dest.IsEvenSemester, opt => opt.MapFrom(src => src.isEvenSemester))
                .ForMember(dest => dest.Disciplines, opt => opt.MapFrom(src => src.disciplines));

            CreateMap<(SelectiveDiscipline discipline, SelectiveDetail details), FullDisciplineWithDetailsDto>()
                .ForMember(dest => dest.IdSelectiveDisciplines, opt => opt.MapFrom(src => src.discipline.IdSelectiveDisciplines))
                .ForMember(dest => dest.NameSelectiveDisciplines, opt => opt.MapFrom(src => src.discipline.NameSelectiveDisciplines))
                .ForMember(dest => dest.CodeSelectiveDisciplines, opt => opt.MapFrom(src => src.discipline.CodeSelectiveDisciplines))
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.discipline.Department.Faculty.Abbreviation))
                .ForMember(dest => dest.MinCountPeople, opt => opt.MapFrom(src => src.discipline.MinCountPeople))
                .ForMember(dest => dest.MaxCountPeople, opt => opt.MapFrom(src => src.discipline.MaxCountPeople))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.discipline.Courses))
                .ForMember(dest => dest.IsEven, opt => opt.MapFrom(src => src.discipline.IsEven))
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.discipline.DegreeLevel.NameEducationalDegree))
                .ForMember(dest => dest.NameSelectiveDisciplinesEng, opt => opt.MapFrom(src => src.details.NameSelectiveDisciplinesEng))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.discipline.Department.NameDepartment))
                .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.details.Teachers))
                .ForMember(dest => dest.Recomend, opt => opt.MapFrom(src => src.details.Recommended))
                .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src => src.details.Prerequisites))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.details.Language))
                .ForMember(dest => dest.Provision , opt => opt.MapFrom(src => src.details.Provision))
                .ForMember(dest => dest.WhyInterestingDetermination, opt => opt.MapFrom(src => src.details.WhyInterestingDetermination))
                .ForMember(dest => dest.ResultEducation, opt => opt.MapFrom(src => src.details.ResultEducation))
                .ForMember(dest => dest.UsingIrl, opt => opt.MapFrom(src => src.details.UsingIrl))
                .ForMember(dest => dest.DisciplineTopics, opt => opt.MapFrom(src => src.details.DisciplineTopics))
                .ForMember(dest => dest.TypesOfTraining, opt => opt.MapFrom(src => src.details.TypesOfTraining))
                .ForMember(dest => dest.TypeOfControl, opt => opt.MapFrom(src => src.discipline.TypeOfControl.Type))
                .ForMember(dest => dest.CatalogId, opt => opt.MapFrom(src => src.discipline.CatalogId))
                .ForMember(dest => dest.ApprovalStatusId, opt => opt.MapFrom(src => src.discipline.ApprovalStatusId))
                .ForMember(dest => dest.TypeOfControlId, opt => opt.MapFrom(src => src.discipline.TypeOfControlId));

            //Department
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.NameFaculty));
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            //BindLoansMain
            CreateMap<BindLoansMain, BindLoansMainDto>()
                .ForMember(dest => dest.SelectiveDisciplineName, opt => opt.MapFrom(src => src.SelectiveDisciplines.NameSelectiveDisciplines))
                .ForMember(dest => dest.CodeSelectiveDisciplines, opt => opt.MapFrom(src => src.SelectiveDisciplines.CodeSelectiveDisciplines))
                .ForMember(dest => dest.SpecialityCode, opt => opt.MapFrom(src => src.EducationalProgram != null && src.EducationalProgram.Speciality != null && src.EducationalProgram.Speciality.Code.Length > 0 ? src.EducationalProgram.Speciality.Code : ""))
                .ForMember(dest => dest.EducationalProgramName, opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram));
            CreateMap<CreateBindLoansMainDto, BindLoansMain>();
            CreateMap<UpdateBindLoansMainDto, BindLoansMain>();

            //SelectiveDetail
            CreateMap<DetailContentDto, SelectiveDetail>()
                .ForMember(dest => dest.DisciplineTopics, opt => opt.MapFrom(src => src.DisciplineTopics != null ? JsonSerializer.Serialize(src.DisciplineTopics, (JsonSerializerOptions)null) : null));

            CreateMap<CreateSelectiveDetailDto, SelectiveDetail>()
                .IncludeMembers(src => src.Content);

            CreateMap<SelectiveDetail, DetailContentDto>()
                .ForMember(dest => dest.DisciplineTopics, opt => opt.MapFrom(src => src.DisciplineTopics.Count > 0 ? src.DisciplineTopics : null))
                .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.Teachers))
                .ForMember(dest => dest.Recomend, opt => opt.MapFrom(src => src.Recommended));

            CreateMap<SelectiveDetail, SelectiveDetailDto>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src));


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
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.IdPermissions, opt => opt.MapFrom(src => src.IdPermission))
                .ForMember(dest => dest.TypePermission, opt => opt.MapFrom(_ => "S"))
                .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Code));
            CreateMap<PermissionDto, Permission>()
                .ForMember(dest => dest.IdPermission, opt => opt.MapFrom(src => src.IdPermissions))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => $"{src.TypePermission}:{src.TableName}"));
            CreateMap<CreatePermissionDto, Permission>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => $"{src.TypePermission}:{src.TableName}"))
                .ForMember(dest => dest.IdPermission, opt => opt.Ignore());
            CreateMap<UpdatePermissionDto, Permission>()
                .ForMember(dest => dest.IdPermission, opt => opt.MapFrom(src => src.IdPermissions))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => $"{src.TypePermission}:{src.TableName}"));

            //BindRolePermission
            CreateMap<RolePermission, BindRolePermissionDto>()
                .ForMember(dest => dest.IdBindRolePermission, opt => opt.MapFrom(src => src.PermissionId))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty))
                .ForMember(dest => dest.TypePermission, opt => opt.MapFrom(src => src.Permission != null ? "S" : string.Empty))
                .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Permission != null ? src.Permission.Code : string.Empty));
            CreateMap<CreateBindRolePermissionDto, RolePermission>();
            CreateMap<UpdateBindRolePermissionDto, RolePermission>();

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
