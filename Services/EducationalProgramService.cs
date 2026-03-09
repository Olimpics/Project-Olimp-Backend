using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;

namespace OlimpBack.Services;

public class EducationalProgramService : IEducationalProgramService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EducationalProgramService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<EducationalProgramDto>> GetEducationalProgramsAsync(EducationalProgramListQueryDto queryDto)
    {
        var query = _context.EducationalPrograms
            .Include(ep => ep.Degree)
            .Include(ep => ep.Students)
            .Include(ep => ep.BindMainDisciplines)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(ep =>
                EF.Functions.Like(ep.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(ep.SpecialityCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds))
        {
            var degreeLevelIdList = queryDto.DegreeLevelIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (degreeLevelIdList.Any())
            {
                query = query.Where(ep => degreeLevelIdList.Contains(ep.DegreeId));
            }
        }

        query = queryDto.SortOrder switch
        {
            1 => query.OrderBy(ep => ep.NameEducationalProgram),
            2 => query.OrderByDescending(ep => ep.NameEducationalProgram),
            3 => query.OrderByDescending(ep => ep.SpecialityCode),
            4 => query.OrderBy(ep => ep.StudentsAmount),
            5 => query.OrderByDescending(ep => ep.StudentsAmount),
            _ => query.OrderBy(ep => ep.SpecialityCode)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        var programs = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        var result = programs
            .Select(ep => _mapper.Map<EducationalProgramDto>(ep))
            .ToList();
        return new PaginatedResponseDto<EducationalProgramDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = result
        };
    }

    public async Task<EducationalProgramDto?> GetEducationalProgramAsync(int id)
    {
        var program = await _context.EducationalPrograms
            .Include(ep => ep.Degree)
            .Include(ep => ep.Students)
            .Include(ep => ep.BindMainDisciplines)
            .FirstOrDefaultAsync(ep => ep.IdEducationalProgram == id);

        if (program == null)
            return null;

        return _mapper.Map<EducationalProgramDto>(program);
    }

    public async Task<EducationalProgramDto> CreateEducationalProgramAsync(CreateEducationalProgramDto dto)
    {
        var program = _mapper.Map<Models.EducationalProgram>(dto);
        program.DegreeId = int.Parse(dto.Degree);

        _context.EducationalPrograms.Add(program);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<EducationalProgramDto>(program);
        result.StudentsCount = 0;

        return result;
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateEducationalProgramAsync(
        int id,
        UpdateEducationalProgramDto dto)
    {
        var program = await _context.EducationalPrograms.FindAsync(id);
        if (program == null)
        {
            return (false, StatusCodes.Status404NotFound, "Educational program not found");
        }

        _mapper.Map(dto, program);
        program.DegreeId = int.Parse(dto.Degree);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.EducationalPrograms.AnyAsync(e => e.IdEducationalProgram == id);
            if (!exists)
            {
                return (false, StatusCodes.Status404NotFound, "Educational program not found");
            }

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteEducationalProgramAsync(int id)
    {
        var program = await _context.EducationalPrograms.FindAsync(id);
        if (program == null)
        {
            return (false, StatusCodes.Status404NotFound, "Educational program not found");
        }

        _context.EducationalPrograms.Remove(program);
        await _context.SaveChangesAsync();

        return (true, StatusCodes.Status204NoContent, null);
    }
}
