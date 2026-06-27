using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.DTO.Sg;
using OlimpBack.Models;

namespace OlimpBack.MappingProfiles
{
    
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // SG Mappings
            CreateMap<SubDivisionsSg, SubDivisionUserDto>()
                .ForMember(dest => dest.SubDivisionId, opt => opt.MapFrom(src => src.IdSubDivisions));

            CreateMap<MembersOfSg, SubDivisionUserDto>()
                .ForMember(dest => dest.SubDivisionId, opt => opt.MapFrom(src => src.BindSubdivisionRoleInSgNavigation.SubDivisionId))
                .ForMember(dest => dest.NameDivision, opt => opt.MapFrom(src => src.BindSubdivisionRoleInSgNavigation.SubDivision.NameDivision))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
                .ForMember(dest => dest.Abbreviation, opt => opt.MapFrom(src => src.Faculty != null ? src.Faculty.Abbreviation : null));

            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<EventCreateUpdateDto, Event>();

            CreateMap<MembersOfSg, StudentSgDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Student.Group.GroupCode))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty != null ? src.Faculty.NameFaculty : src.Student.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty))
                .ForMember(dest => dest.RoleInSg, opt => opt.MapFrom(src => src.BindSubdivisionRoleInSgNavigation.RoleInSg.NameRole));

            CreateMap<BindEventStudent, BindEventStudentDto>().ReverseMap();
            CreateMap<BindEventStudentCreateUpdateDto, BindEventStudent>();

            CreateMap<MembersOfSg, MemberSgDto>().ReverseMap();
            CreateMap<MemberSgCreateUpdateDto, MembersOfSg>();

            CreateMap<InventorySg, InventorySgDto>().ReverseMap();
            CreateMap<InventorySgCreateUpdateDto, InventorySg>();

            CreateMap<AccountingJournal, AccountingJournalDto>().ReverseMap();
            CreateMap<AccountingJournalCreateUpdateDto, AccountingJournal>();

            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.IdUser, opt => opt.Ignore());

            CreateMap<User, UpdateUserDto>();

            // Login mapping
             CreateMap<Student, LoginResponseStudentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdStudent))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.ThirdName))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.FacultyId))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Name))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Group.Course))
                .ForMember(dest => dest.DegreeLevel, opt => opt.MapFrom(src => src.Group.EducationalProgram.Degree.NameEducationalDegree));


            CreateMap<AdminsPersonal, LoginResponseAdminDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.ThirdName))
                .ForMember(dest => dest.NameFaculty, opt => opt.MapFrom(src => src.Faculty.NameFaculty))
                .ForMember (dest => dest.IsAdmin, opt => opt.MapFrom(src => true));


            //Student
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.EducationStatus.NameEducationStatus))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.Faculty.NameFaculty))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Group.EducationalProgram.NameEducationalProgram))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.Group.EducationalProgram.Degree.NameEducationalDegree))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupCode));

            CreateMap<Student, StudentForCatalogDto>()
                .ForMember(dest => dest.FacultyAbbreviation, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation)) 
                .ForMember(dest => dest.SpecialityCode, opt => opt.MapFrom(src => src.Group.EducationalProgram != null && src.Group.EducationalProgram.Speciality != null && src.Group.EducationalProgram.Speciality.Code.Length > 0 ? src.Group.EducationalProgram.Speciality.Code : ""))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.Group.EducationalProgram.Speciality.Name))
                .ForMember(dest => dest.DegreeName, opt => opt.MapFrom(src => src.Group.EducationalProgram != null && src.Group.EducationalProgram.Degree != null ? src.Group.EducationalProgram.Degree.NameEducationalDegree : ""))
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

           

            // BindSelectiveDiscipline
            CreateMap<BindSelectiveDiscipline, BindSelectiveDisciplineDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Student.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.Student.SecondName))
                .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.Student.ThirdName))
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
            CreateMap<EducationalDegree, EducationalDegreeDto>();
            CreateMap<CreateEducationalDegreeDto, EducationalDegree>();
            CreateMap<UpdateEducationalDegreeDto, EducationalDegree>();

            //EducationalProgram
            CreateMap<EducationalProgram, EducationalProgramDto>()
                .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree.NameEducationalDegree))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Speciality.Department.NameDepartment))
                .ForMember(dest => dest.Faculty, opt => opt.MapFrom(src => src.Speciality.Department.Faculty.Abbreviation))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.Speciality.Name))
                .ForMember(dest => dest.StudyForm, opt => opt.MapFrom(src => src.StudyForm.NameStudyForm))
                .ForMember(dest => dest.IsAccelerated, opt => opt.MapFrom(src => src.IsAccelerated ? "Yes" : "No"));                

            CreateMap<EducationalProgram, EducationalProgramFullDto>()
                .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree.NameEducationalDegree))
                .ForMember(dest => dest.DegreeLevelName, opt => opt.MapFrom(src => src.Degree.NameEducationalDegree))
                .ForMember(dest => dest.Catalog, opt => opt.MapFrom(src => src.Catalog != null ? new CatalogDto { StartYear = src.Catalog.YearStart, EndYear = src.Catalog.YearEnd } : null))
                .ForMember(dest => dest.SpecializationName, opt => opt.MapFrom(src => src.Specialization != null ? src.Specialization.Name : null))
                .ForMember(dest => dest.SpecialityName, opt => opt.MapFrom(src => src.Speciality.Name))
                .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.Speciality.Name))
                .ForMember(dest => dest.StudyFormName, opt => opt.MapFrom(src => src.StudyForm.NameStudyForm))
                .ForMember(dest => dest.SelectiveDisciplineBySemestr, opt => opt.MapFrom(src => src.StudentGroups.SelectMany(g => g.Students).SelectMany(s => s.BindSelectiveDisciplines).GroupBy(b => b.SelectiveDisciplineId).Select(g => new { SelectiveDisciplineId = g.Key, Semesters = g.Select(b => b.Semestr) }).ToDictionary(x => x.SelectiveDisciplineId, x => x.Semesters)))
                .ForMember(dest => dest.MinUniSelectiveDisciplineBySemestr, opt => opt.MapFrom(src => src.MinUniSelectiveDisciplineBySemestr))
                .ForMember(dest => dest.IsAccelerated, opt => opt.MapFrom(src => src.IsAccelerated ? "Yes" : "No"))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.Goals))
                .ForMember(dest => dest.Keys, opt => opt.MapFrom(src => src.Keys != null ? src.Keys : null));



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
                .ForMember(dest => dest.IdCatalogYear, opt => opt.MapFrom(src => src.IdCatalogYearSelective))
                .ForMember(dest => dest.IsFormed, opt => opt.MapFrom(src => src.IsFormed))
                .ForMember(dest => dest.NameCatalog, opt => opt.MapFrom(src => $"{src.YearStart}-{src.YearEnd}"))
                .ForMember(dest => dest.yearStart, opt => opt.MapFrom(src => src.YearStart))
                .ForMember(dest => dest.yearEnd, opt => opt.MapFrom(src => src.YearEnd));
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
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedInFaculty, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedInDepartment, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedInGroup, opt => opt.Ignore());

            //StudyForm
            CreateMap<StudyForm, StudyFormDto>().ReverseMap();
          

            //DisciplineTab
            CreateMap<(Student student, List<SelectiveDiscipline> disciplines, int currentCourse, bool isEvenSemester), DisciplineTabResponseDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.student.IdStudent))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.student.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.student.SecondName))
                .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.student.ThirdName))
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
                .ForMember(dest => dest.Recommended, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.details.Recommended) ? System.Text.Json.JsonSerializer.Deserialize<FullDisciplineWithDetailsDto.RecommendedDto>(src.details.Recommended, (System.Text.Json.JsonSerializerOptions?)null) : null))
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
                .ForMember(dest => dest.TypeOfControlId, opt => opt.MapFrom(src => src.discipline.TypeOfControlId))
                .ForMember(dest => dest.Feedback, opt => opt.MapFrom(src => src.discipline.Feedback))
                .ForMember(dest => dest.IsForseChange, opt => opt.MapFrom(src => src.discipline.IsForseChange))
                .ForMember(dest => dest.NameDock, opt => opt.MapFrom(src => src.discipline.NameDock))
                .ForMember(dest => dest.Keys, opt => opt.MapFrom(src => src.discipline.Keys))
                .ForMember(dest => dest.ApprovalStatus, opt => opt.MapFrom(src => src.discipline.ApprovalStatus.AppovalStatus))
                .ForMember(dest => dest.NeedFix, opt => opt.MapFrom(src => src.discipline.NeedFix))
                .ForMember(dest => dest.YearStart, opt => opt.MapFrom(src => src.discipline.Catalog.YearStart))
                .ForMember(dest => dest.YearEnd, opt => opt.MapFrom(src => src.discipline.Catalog.YearEnd));

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
                .ForMember(dest => dest.DisciplineTopics, opt => opt.MapFrom(src => src.DisciplineTopics));

            CreateMap<CreateSelectiveDetailDto, SelectiveDetail>()
                .IncludeMembers(src => src.Content);

            CreateMap<SelectiveDetail, DetailContentDto>()
                .ForMember(dest => dest.DisciplineTopics, opt => opt.MapFrom(src => src.DisciplineTopics))
                .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.Teachers))
                .ForMember(dest => dest.Recommended, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Recommended) ? System.Text.Json.JsonSerializer.Deserialize<FullDisciplineWithDetailsDto.RecommendedDto>(src.Recommended, (System.Text.Json.JsonSerializerOptions?)null) : null));

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

            //TypeOfControl
            CreateMap<TypeOfControl, TypeOfControlDto>();
            CreateMap<CreateTypeOfControlDto, TypeOfControl>();
            CreateMap<UpdateTypeOfControlDto, TypeOfControl>();
            CreateMap<TypeOfControl, TypeOfControlFilterDto>();

            //Approval
            CreateMap<Approval, ApprovalDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<CreateApprovalDto, Approval>();
            CreateMap<UpdateApprovalDto, Approval>();
            CreateMap<Approval, ApprovalFilterDto>();

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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdDisciplineChoicePeriod))
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.Department.FacultyId))
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.PeriodType ? (sbyte)1 : (sbyte)0))
                .ForMember(dest => dest.PeriodCourse, opt => opt.MapFrom(src => (sbyte)src.PeriodCourse))
                .ForMember(dest => dest.isShort, opt => opt.MapFrom(src => src.IsShort))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.EndOfCheckPeriod, opt => opt.MapFrom(src => src.EndOfCheckPeriod.ToDateTime(TimeOnly.MinValue)));

            CreateMap<CreateDisciplineChoicePeriodDto, DisciplineChoicePeriod>()
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.PeriodType != 0))
                .ForMember(dest => dest.PeriodCourse, opt => opt.MapFrom(src => (int)src.PeriodCourse))
                .ForMember(dest => dest.IsShort, opt => opt.MapFrom(src => src.IsShort))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate ?? DateTime.UtcNow)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndDate ?? DateTime.UtcNow.AddDays(7))))
                .ForMember(dest => dest.EndOfCheckPeriod, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndOfCheckPeriod ?? (src.EndDate ?? DateTime.UtcNow.AddDays(7)).AddDays(1))))
                .ForMember(dest => dest.IsClose, opt => opt.MapFrom(src => false));

            CreateMap<UpdateDisciplineChoicePeriodDto, DisciplineChoicePeriod>()
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.PeriodType != 0))
                .ForMember(dest => dest.PeriodCourse, opt => opt.MapFrom(src => (int)src.PeriodCourse))
                .ForMember(dest => dest.IsShort, opt => opt.MapFrom(src => src.isShort))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate ?? DateTime.UtcNow)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndDate ?? DateTime.UtcNow)))
                .ForMember(dest => dest.EndOfCheckPeriod, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndOfCheckPeriod ?? (src.EndDate ?? DateTime.UtcNow).AddDays(1))))
                .ForMember(dest => dest.IsClose, opt => opt.Ignore());

            CreateMap<UpdateDisciplineChoicePeriodAfterStartDto, DisciplineChoicePeriod>()
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? DateOnly.FromDateTime(src.EndDate.Value) : default))
                .ForMember(dest => dest.EndOfCheckPeriod, opt => opt.MapFrom(src => src.EndOfCheckPeriod.HasValue ? DateOnly.FromDateTime(src.EndOfCheckPeriod.Value) : default))
                .ForMember(dest => dest.IsClose, opt => opt.Ignore());

            CreateMap<UpdateDisciplineChoicePeriodOpenOrCloseDto, DisciplineChoicePeriod>()
                .ForMember(dest => dest.IsClose, opt => opt.Ignore());

        }

    }

}
