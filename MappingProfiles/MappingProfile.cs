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
            ////User
            //CreateMap<User, UserRoleDto>()
            //    .ForMember(dest => dest.IdUsers, opt => opt.MapFrom(src => src.IdUser))
            //    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src =>
            //        src.UserRoles
            //            .OrderBy(ur => ur.RoleId)
            //            .Select(ur => ur.Role.Name)
            //            .FirstOrDefault() ?? string.Empty));
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.IdUser, opt => opt.Ignore());

            CreateMap<User, UpdateUserDto>();

            // Login mapping
             CreateMap<Student, LoginResponseStudentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdStudent))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                //.ForMember(dest => dest.RoleId, opt => opt.MapFrom(src =>
                   // src.User.UserRoles
                     //   .OrderBy(ur => ur.RoleId)
                       // .Select(ur => ur.RoleId)
                        //.FirstOrDefault()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameStudent))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Faculty.NameFaculty))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.EducationalProgram.Speciality))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.DegreeLevel, opt => opt.MapFrom(src => src.EducationalDegree.NameEducationalDegree));


            CreateMap<AdminsPersonal, LoginResponseAdminDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
               // .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src =>
                 //   src.User!.UserRoles
                   //     .OrderBy(ur => ur.RoleId)
                     //   .Select(ur => ur.RoleId)
                       // .FirstOrDefault()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameAdmin))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Faculty.NameFaculty));


            //Student
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.EducationStatus.NameEducationStatus))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.NameFaculty))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.EducationalDegree.NameEducationalDegree))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupCode))
                .ForMember(dest => dest.StudyFormName, opt => opt.MapFrom(src => src.StudyForm.NameStudyForm));

            CreateMap<Student, StudentForCatalogDto>()
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.Faculty.Abbreviation)) 
                .ForMember(dest => dest.SpecialityCode, opt => opt.MapFrom(src => src.EducationalProgram != null && src.EducationalProgram.Speciality != null && src.EducationalProgram.Speciality.Code.HasValue ? src.EducationalProgram.Speciality.Code.Value.ToString() : ""))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.EducationalProgram.Speciality))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.EducationalDegree.NameEducationalDegree))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupCode));

            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();

            //SelectiveDisciplines
            CreateMap<SelectiveDiscipline, SelectiveDisciplineDto>()
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegree));

            CreateMap<SelectiveDiscipline, FullDisciplineDto>()
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegree))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.Faculty.Abbreviation));

            CreateMap<SelectiveDiscipline, FullForAdminDisciplineDto>()
              .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.DegreeLevel.NameEducationalDegree));
            CreateMap<CreateSelectiveDisciplineDto, SelectiveDiscipline>()
                .ForMember(dest => dest.DegreeLevelId, opt => opt.MapFrom(src => src.DegreeLevelId));
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
                           opt => opt.MapFrom(src => src.SelectiveDisciplines.NameSelectiveDisciplines))
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(src => src.InProcess != null && src.InProcess.Length > 0 && src.InProcess[0]));

            CreateMap<CreateBindSelectiveDisciplineDto, BindSelectiveDiscipline>()
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(_ => 1));

            CreateMap<UpdateBindSelectiveDisciplineDto, BindSelectiveDiscipline>()
                .ForMember(dest => dest.InProcess,
                           opt => opt.MapFrom(_ => 1));

            // Normative mapping
            CreateMap<Normative, NormativeDto>()
                // ���������� �� null, ���� ������ �� �������
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
                       opt => opt.MapFrom(src => src.Students.Count));

            CreateMap<CreateEducationalDegreeDto, EducationalDegree>();
            CreateMap<UpdateEducationalDegreeDto, EducationalDegree>();

            //EducationalProgram
            CreateMap<EducationalProgram, EducationalProgramDto>()
          .ForMember(dest => dest.StudentsCount,
                     opt => opt.MapFrom(src => src.Students.Count))
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
                .ForMember(dest => dest.IsFormed, opt => opt.MapFrom(src => src.IsFormed != null && src.IsFormed.Cast<bool>().FirstOrDefault()));
            CreateMap<CreateCatalogYearMainDto, CatalogYearsMain>();
            CreateMap<UpdateCatalogYearMainDto, CatalogYearsMain>();

            //CatalogYearSelective
            CreateMap<CatalogYearsSelective, CatalogYearSelectiveDto>()
                .ForMember(dest => dest.IsFormed, opt => opt.MapFrom(src => src.IsFormed != null && src.IsFormed.Cast<bool>().FirstOrDefault()));
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
                //.ForMember(dest => dest.ChildRoles, opt => opt.Ignore())
                //.ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
                //.ForMember(dest => dest.Permissions, opt => opt.Ignore());
                //.ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                //.ForMember(dest => dest.Users, opt => opt.Ignore());

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
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.discipline.Faculty.Abbreviation))
                .ForMember(dest => dest.MinCountPeople, opt => opt.MapFrom(src => src.discipline.MinCountPeople))
                .ForMember(dest => dest.MaxCountPeople, opt => opt.MapFrom(src => src.discipline.MaxCountPeople))
                .ForMember(dest => dest.MinCourse, opt => opt.MapFrom(src => src.discipline.MinCourse))
                .ForMember(dest => dest.MaxCourse, opt => opt.MapFrom(src => src.discipline.MaxCourse))
                .ForMember(dest => dest.IsEven, opt => opt.MapFrom(src => src.discipline.IsEven))
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.discipline.DegreeLevel.NameEducationalDegree))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.details.Department.NameDepartment))
                .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.details.Teachers))
                .ForMember(dest => dest.Recomend, opt => opt.MapFrom(src => src.details.Recomend))
                .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src => src.details.Prerequisites))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.details.Language))
                .ForMember(dest => dest.Provision , opt => opt.MapFrom(src => src.details.Provision))
                .ForMember(dest => dest.WhyInterestingDetermination, opt => opt.MapFrom(src => src.details.WhyInterestingDetermination))
                .ForMember(dest => dest.ResultEducation, opt => opt.MapFrom(src => src.details.ResultEducation))
                .ForMember(dest => dest.UsingIrl, opt => opt.MapFrom(src => src.details.UsingIrl))
                .ForMember(dest => dest.DisciplineTopics, opt => opt.MapFrom(src => src.details.DisciplineTopics))
                .ForMember(dest => dest.TypesOfTraining, opt => opt.MapFrom(src => src.details.TypesOfTraining))
                .ForMember(dest => dest.TypeOfControl, opt => opt.MapFrom(src => src.details.TypeOfControll.Type));

            //Department
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.NameFaculty));
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            //BindLoansMain
            CreateMap<BindLoansMain, BindLoansMainDto>()
                .ForMember(dest => dest.SelectiveDisciplineName, opt => opt.MapFrom(src => src.SelectiveDisciplines.NameSelectiveDisciplines))
                .ForMember(dest => dest.CodeSelectiveDisciplines, opt => opt.MapFrom(src => src.SelectiveDisciplines.CodeSelectiveDisciplines))
                .ForMember(dest => dest.SpecialityCode, opt => opt.MapFrom(src => src.EducationalProgram != null && src.EducationalProgram.Speciality != null && src.EducationalProgram.Speciality.Code.HasValue ? src.EducationalProgram.Speciality.Code.Value.ToString() : ""))
                .ForMember(dest => dest.EducationalProgramName, opt => opt.MapFrom(src => src.EducationalProgram.NameEducationalProgram));
            CreateMap<CreateBindLoansMainDto, BindLoansMain>();
            CreateMap<UpdateBindLoansMainDto, BindLoansMain>();

            //SelectiveDetail
            CreateMap<DetailContentDto, SelectiveDetail>();

            // 2. ����� ������ CreateSelectiveDetailDto �� SelectiveDetail
            CreateMap<CreateSelectiveDetailDto, SelectiveDetail>()
                // ������: "�������� ���������� Content � ����� ���� �����"
                .IncludeMembers(src => src.Content);

            // 3. � ��� ���� � ��������� �� (���� ����� ������ � �� � DTO)
            CreateMap<SelectiveDetail, DetailContentDto>();

            CreateMap<SelectiveDetail, SelectiveDetailDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.NameDepartment))
                // ������: "������� �� ������� ���� � ���������� Content"
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
