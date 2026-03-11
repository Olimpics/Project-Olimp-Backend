using AutoMapper;
using AutoMapper.QueryableExtensions; // Додано для ProjectTo
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
        // Відключаємо трекінг, Include нам більше не потрібні завдяки ProjectTo!
        var query = _context.Groups.AsNoTracking().AsQueryable();

        // 1. ФІЛЬТРАЦІЯ
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(g => EF.Functions.Like(g.GroupCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.FacultyIds != null && queryDto.FacultyIds.Any())
        {
            query = query.Where(g => g.FacultyId.HasValue && queryDto.FacultyIds.Contains(g.FacultyId.Value));
        }

        if (queryDto.DepartmentIds != null && queryDto.DepartmentIds.Any())
        {
            query = query.Where(g => g.DepartmentId.HasValue && queryDto.DepartmentIds.Contains(g.DepartmentId.Value));
        }

        if (queryDto.Courses != null && queryDto.Courses.Any())
        {
            query = query.Where(g => g.Course.HasValue && queryDto.Courses.Contains(g.Course.Value));
        }

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(g => g.DegreeId.HasValue && queryDto.DegreeLevelIds.Contains(g.DegreeId.Value));
        }

        // 2. СОРТУВАННЯ НА РІВНІ БД (До ToListAsync!)
        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(d => d.GroupCode),
            2 => query.OrderByDescending(d => d.GroupCode),
            3 => query.OrderBy(d => d.Faculty.Abbreviation), // EF Core сам зробить JOIN, якщо треба
            4 => query.OrderByDescending(d => d.Faculty.Abbreviation),
            5 => query.OrderBy(d => d.Course),
            6 => query.OrderByDescending(d => d.Course),
            _ => query.OrderBy(d => d.GroupCode)
        };

        // 3. БЛИСКАВИЧНА ПРОЕКЦІЯ
        // SQL-запит витягне лише ті колонки, які вказані в GroupFilterDto
        return await query
            .ProjectTo<GroupFilterDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<GroupDto?> GetGroupAsync(int id)
    {
        // Видалено Include, використано AsNoTracking та ProjectTo
        return await _context.Groups
            .AsNoTracking()
            .Where(g => g.IdGroup == id)
            .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
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
            return (false, StatusCodes.Status404NotFound, "Group not found");

        _mapper.Map(dto, group);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Groups.AnyAsync(g => g.IdGroup == id);
            if (!exists)
                return (false, StatusCodes.Status404NotFound, "Group not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteGroupAsync(int id)
    {
        // 4. СУЧАСНЕ ВИДАЛЕННЯ БЕЗ ЗАВАНТАЖЕННЯ (EF Core 7+)
        var deletedRows = await _context.Groups
            .Where(g => g.IdGroup == id)
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Group not found");

        return (true, StatusCodes.Status204NoContent, null);
    }
}