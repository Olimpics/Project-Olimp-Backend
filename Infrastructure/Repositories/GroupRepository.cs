using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IGroupRepository
{
    Task<IEnumerable<GroupFilterDto>> GetFilteredGroupsAsync(GroupListQueryDto queryDto);
    Task<GroupDto?> GetDtoByIdAsync(int id);
    Task<GroupDetailsDto?> GetDetailsByIdAsync(int id);
    Task<IReadOnlyList<GroupStudentDto>> GetStudentsByGroupIdAsync(int groupId);
    Task<StudentGroup?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(StudentGroup group);
    Task<int> DeleteAsync(int id);

    Task<GroupCurriculumDTO?> GetCurriculumByGroupIdAsync(int groupId);
    Task SaveChangesAsync();
}

public class GroupRepository : IGroupRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GroupRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GroupFilterDto>> GetFilteredGroupsAsync(GroupListQueryDto queryDto)
    {
        var query = _context.StudentGroups.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(g => EF.Functions.Like(g.GroupCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.FacultyIds != null && queryDto.FacultyIds.Any())
            query = query.Where(g => g.EducationalProgram != null && 
                                     g.EducationalProgram.Speciality != null && 
                                     g.EducationalProgram.Speciality.Department != null &&
                                     g.EducationalProgram.Speciality.Department.FacultyId.HasValue &&
                                     queryDto.FacultyIds.Contains(g.EducationalProgram.Speciality.Department.FacultyId.Value));
        if (queryDto.DepartmentIds != null && queryDto.DepartmentIds.Any())
            query = query.Where(g => g.EducationalProgram != null && 
                                     g.EducationalProgram.Speciality != null && 
                                     g.EducationalProgram.Speciality.Department != null &&
                                     g.EducationalProgram.Speciality.Department.IdDepartment !=0 &&
                                     queryDto.DepartmentIds.Contains(g.EducationalProgram.Speciality.Department.IdDepartment));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(g => g.Course.HasValue && queryDto.Courses.Contains(g.Course.Value));

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(g => g.DegreeId.HasValue && queryDto.DegreeLevelIds.Contains(g.DegreeId.Value));

        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(d => d.GroupCode),
            2 => query.OrderByDescending(d => d.GroupCode),
            3 => query.OrderBy(d => d.EducationalProgram.Speciality.Department.Faculty.Abbreviation),
            4 => query.OrderByDescending(d => d.EducationalProgram.Speciality.Department.Faculty.Abbreviation),
            5 => query.OrderBy(d => d.Course),
            6 => query.OrderByDescending(d => d.Course),
            _ => query.OrderBy(d => d.GroupCode)
        };

        return await query
            .ProjectTo<GroupFilterDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<GroupDto?> GetDtoByIdAsync(int id)
    {
        return await _context.StudentGroups
            .AsNoTracking()
            .Where(g => g.IdGroup == id)
            .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<GroupDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await _context.StudentGroups
            .AsNoTracking()
            .Where(g => g.IdGroup == id)
            .Select(g => new GroupDetailsDto
            {
                IdGroup = g.IdGroup,
                GroupCode = g.GroupCode,
                NumberOfStudents = g.NumberOfStudents,
                AdminId = g.AdminId,
                DegreeId = g.DegreeId,
                Course = g.Course,
                FacultyId = g.EducationalProgram != null && g.EducationalProgram.Speciality != null && g.EducationalProgram.Speciality.Department != null 
                            ? g.EducationalProgram.Speciality.Department.FacultyId : null,
                FacultyName = g.EducationalProgram != null && g.EducationalProgram.Speciality != null && g.EducationalProgram.Speciality.Department != null && g.EducationalProgram.Speciality.Department.Faculty != null
                            ? g.EducationalProgram.Speciality.Department.Faculty.NameFaculty : null,
                DepartmentId = g.EducationalProgram != null && g.EducationalProgram.Speciality != null 
                             ? g.EducationalProgram.Speciality.DepartmentId : null,
                DepartmentName = g.EducationalProgram != null && g.EducationalProgram.Speciality != null && g.EducationalProgram.Speciality.Department != null
                             ? g.EducationalProgram.Speciality.Department.NameDepartment : null,
                IdEducationalProgram = g.EducationalProgramId,
                EducationalProgramName = g.EducationalProgram != null ? g.EducationalProgram.NameEducationalProgram : null,
                IdSpeciality = g.EducationalProgram != null ? g.EducationalProgram.SpecialityId : null,
                SpecialityName = g.EducationalProgram != null && g.EducationalProgram.Speciality != null 
                               ? g.EducationalProgram.Speciality.Name : null,
                AdmissionYear = g.AdmissionYear.HasValue ? g.AdmissionYear.Value.Year : null,
                IdStudyForm = g.StudyFormId,
                IsAccelerated = g.IsAccelerated.Cast<bool>().FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<GroupStudentDto>> GetStudentsByGroupIdAsync(int groupId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.GroupId == groupId)
            .OrderBy(s => s.NameStudent)
            .Select(s => new GroupStudentDto
            {
                IdStudent = s.IdStudent,
                UserId = s.UserId,
                NameStudent = s.NameStudent,
                EmailStudent = s.User.Email,
                EductionalStatus = s.EducationStatus.NameEducationStatus
            })
            .ToListAsync();
    }

    public async Task<GroupCurriculumDTO?> GetCurriculumByGroupIdAsync(int groupId)
    {
        return await _context.StudentGroups
            .AsNoTracking()
            .Where(g => g.IdGroup == groupId)
            .Select(g => new GroupCurriculumDTO
            {
                IdGroup = g.IdGroup,
                GroupCode = g.GroupCode,
                MainDisciplines = _context.MainDisciplines
                    .Where(bmd => bmd.EducationalProgramId == g.EducationalProgramId)
                    .Select(bmd => new GroupMainDisciplineDto
                    {
                        idMainDisciplines = bmd.IdBindMainDisciplines,
                        nameMainDisciplines = bmd.NameBindMainDisciplines,
                        Semestr = bmd.Semestr,
                        Loans = bmd.Loans,
                        Hours = bmd.Hours,
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StudentGroup?> GetEntityByIdAsync(int id) =>
        await _context.StudentGroups.FindAsync(id);

    public async Task<bool> ExistsAsync(int id) =>
        await _context.StudentGroups.AnyAsync(g => g.IdGroup == id);

    public async Task AddAsync(StudentGroup group) =>
        await _context.StudentGroups.AddAsync(group);

    public async Task<int> DeleteAsync(int id) =>
        await _context.StudentGroups.Where(g => g.IdGroup == id).ExecuteDeleteAsync();

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
