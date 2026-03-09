using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;

namespace OlimpBack.Services;

public class GroupService : IGroupService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GroupService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GroupFilterDto>> GetGroupsAsync(GroupListQueryDto queryDto)
    {
        var query = _context.Groups
            .Include(g => g.Students)
            .Include(g => g.Faculty)
            .Include(g => g.Department)
            .Include(g => g.Degree)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(g => EF.Functions.Like(g.GroupCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.FacultyIds))
        {
            var facultyIdList = queryDto.FacultyIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (facultyIdList.Any())
            {
                query = query.Where(g => g.FacultyId.HasValue && facultyIdList.Contains(g.FacultyId.Value));
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.DepartmentIds))
        {
            var departmentIdList = queryDto.DepartmentIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (departmentIdList.Any())
            {
                query = query.Where(g => g.DepartmentId.HasValue && departmentIdList.Contains(g.DepartmentId.Value));
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Courses))
        {
            var courseList = queryDto.Courses
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (courseList.Any())
            {
                query = query.Where(g => g.Course.HasValue && courseList.Contains(g.Course.Value));
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds))
        {
            var degreeLevelIdList = queryDto.DegreeLevelIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (degreeLevelIdList.Any())
            {
                query = query.Where(g => g.DegreeId.HasValue && degreeLevelIdList.Contains(g.DegreeId.Value));
            }
        }

        var groups = await query.ToListAsync();
        groups = queryDto.SortOrder switch
        {
            1 => groups.OrderBy(d => d.GroupCode).ToList(),
            2 => groups.OrderByDescending(d => d.GroupCode).ToList(),
            3 => groups.OrderBy(d => d.Faculty.Abbreviation).ToList(),
            4 => groups.OrderByDescending(d => d.Faculty.Abbreviation).ToList(),
            5 => groups.OrderBy(d => d.Course).ToList(),
            6 => groups.OrderByDescending(d => d.Course).ToList(),
            _ => groups.OrderBy(d => d.GroupCode).ToList()
        };

        return _mapper.Map<IEnumerable<GroupFilterDto>>(groups);
    }

    public async Task<GroupDto?> GetGroupAsync(int id)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.IdGroup == id);

        if (group == null)
            return null;

        return _mapper.Map<GroupDto>(group);
    }

    public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto)
    {
        var group = _mapper.Map<Models.Group>(dto);
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return _mapper.Map<GroupDto>(group);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateGroupAsync(int id, UpdateGroupDto dto)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null)
        {
            return (false, StatusCodes.Status404NotFound, "Group not found");
        }

        _mapper.Map(dto, group);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Groups.AnyAsync(g => g.IdGroup == id);
            if (!exists)
            {
                return (false, StatusCodes.Status404NotFound, "Group not found");
            }

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteGroupAsync(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null)
        {
            return (false, StatusCodes.Status404NotFound, "Group not found");
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        return (true, StatusCodes.Status204NoContent, null);
    }
}

