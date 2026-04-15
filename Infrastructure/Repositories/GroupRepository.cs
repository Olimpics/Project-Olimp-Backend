using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IGroupRepository
{
    Task<IEnumerable<GroupFilterDto>> GetFilteredGroupsAsync(GroupListQueryDto queryDto);
    Task<GroupDto?> GetDtoByIdAsync(int id);
    Task<GroupDetailsDto?> GetDetailsByIdAsync(int id);
    Task<IReadOnlyList<GroupStudentDto>> GetStudentsByGroupIdAsync(int groupId);
    Task<Group?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Group group);
    Task<int> DeleteAsync(int id);
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
        var query = _context.Groups.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(g => EF.Functions.Like(g.GroupCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.FacultyIds != null && queryDto.FacultyIds.Any())
            query = query.Where(g => g.FacultyId.HasValue && queryDto.FacultyIds.Contains(g.FacultyId.Value));

        if (queryDto.DepartmentIds != null && queryDto.DepartmentIds.Any())
            query = query.Where(g => g.DepartmentId.HasValue && queryDto.DepartmentIds.Contains(g.DepartmentId.Value));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(g => g.Course.HasValue && queryDto.Courses.Contains(g.Course.Value));

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(g => g.DegreeId.HasValue && queryDto.DegreeLevelIds.Contains(g.DegreeId.Value));

        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(d => d.GroupCode),
            2 => query.OrderByDescending(d => d.GroupCode),
            3 => query.OrderBy(d => d.Faculty.Abbreviation),
            4 => query.OrderByDescending(d => d.Faculty.Abbreviation),
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
        return await _context.Groups
            .AsNoTracking()
            .Where(g => g.IdGroup == id)
            .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<GroupDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await _context.Groups
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
                FacultyId = g.FacultyId,
                DepartmentId = g.DepartmentId,
                IdEducationalProgram = g.IdEducationalProgram,
                IdSpeciality = g.IdSpeciality,
                AdmissionYear = g.AdmissionYear,
                IdStudyForm = g.IdStudyForm,
                IdSpecialization = g.IdSpecialization,
                IsAccelerated = g.IsAccelerated
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
                Course = s.Course,
                GroupId = s.GroupId
            })
            .ToListAsync();
    }

    public async Task<Group?> GetEntityByIdAsync(int id) =>
        await _context.Groups.FindAsync(id);

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Groups.AnyAsync(g => g.IdGroup == id);

    public async Task AddAsync(Group group) =>
        await _context.Groups.AddAsync(group);

    public async Task<int> DeleteAsync(int id) =>
        await _context.Groups.Where(g => g.IdGroup == id).ExecuteDeleteAsync();

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}