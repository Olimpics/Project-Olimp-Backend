using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;

namespace OlimpBack.Application.Services;

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
        // Відключаємо Tracking одразу, Include більше НЕ ПОТРІБНІ!
        var query = _context.EducationalPrograms.AsNoTracking().AsQueryable();

        // 1. Фільтрація
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(ep =>
                EF.Functions.Like(ep.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(ep.SpecialityCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
        {
            query = query.Where(ep => queryDto.DegreeLevelIds.Contains(ep.DegreeId));
        }

        // 2. Сортування
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

        // 3. МАГІЧНА ПРОЕКЦІЯ
        // EF Core автоматично перетворить .Count() на ефективний SQL: SELECT COUNT(*) ...
        var result = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram,
                DegreeId = ep.DegreeId,
                // Підтягуємо назву рівня, якщо вона є (назва властивості може відрізнятися у твоїй БД)
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegreec : "",
                SpecialityCode = ep.SpecialityCode,
                Speciality = ep.Speciality,
                StudentsAmount = ep.StudentsAmount,
                StudentsCount = ep.Students.Count(), // Жодного вивантаження студентів у пам'ять!
                DisciplinesCount = ep.BindMainDisciplines.Count() // Жодного вивантаження дисциплін!
            })
            .ToListAsync();

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
        // Знову застосовуємо проекцію для одного запису
        var program = await _context.EducationalPrograms
            .AsNoTracking()
            .Where(ep => ep.IdEducationalProgram == id)
            .Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram,
                DegreeId = ep.DegreeId,
                Degree = ep.Degree != null ? ep.Degree.NameEducationalDegreec : "",
                SpecialityCode = ep.SpecialityCode,
                Speciality = ep.Speciality,
                StudentsAmount = ep.StudentsAmount,
                StudentsCount = ep.Students.Count(),
                DisciplinesCount = ep.BindMainDisciplines.Count()
            })
            .FirstOrDefaultAsync();

        return program;
    }

    public async Task<EducationalProgramDto> CreateEducationalProgramAsync(CreateEducationalProgramDto dto)
    {
        var program = _mapper.Map<Models.EducationalProgram>(dto);
        // int.Parse більше не потрібен, AutoMapper сам перекине DegreeId

        _context.EducationalPrograms.Add(program);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<EducationalProgramDto>(program);
        result.StudentsCount = 0;
        result.DisciplinesCount = 0;

        return result;
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateEducationalProgramAsync(
        int id, UpdateEducationalProgramDto dto)
    {
        var program = await _context.EducationalPrograms.FindAsync(id);
        if (program == null)
            return (false, StatusCodes.Status404NotFound, "Educational program not found");

        _mapper.Map(dto, program);
        // int.Parse зник назавжди

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.EducationalPrograms.AnyAsync(e => e.IdEducationalProgram == id);
            if (!exists)
                return (false, StatusCodes.Status404NotFound, "Educational program not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteEducationalProgramAsync(int id)
    {
        // ВИКОРИСТОВУЄМО СУЧАСНЕ ВИДАЛЕННЯ БЕЗ ВИТЯГУВАННЯ З БД (EF Core 7+)
        var deletedRows = await _context.EducationalPrograms
            .Where(ep => ep.IdEducationalProgram == id)
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Educational program not found");

        return (true, StatusCodes.Status204NoContent, null);
    }
}