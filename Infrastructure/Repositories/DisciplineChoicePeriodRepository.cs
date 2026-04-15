using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IDisciplineChoicePeriodRepository
{
    Task<List<DisciplineChoicePeriodDto>> GetAllDtoAsync(GetDisciplineChoicePeriodsQueryDto queryDto);
    Task<DisciplineChoicePeriodDto?> GetDtoByIdAsync(int id);
    Task<DisciplineChoicePeriod?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(DisciplineChoicePeriod period);
    Task SaveChangesAsync();
}

public class DisciplineChoicePeriodRepository : IDisciplineChoicePeriodRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DisciplineChoicePeriodRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<DisciplineChoicePeriodDto>> GetAllDtoAsync(GetDisciplineChoicePeriodsQueryDto queryDto)
    {
        var query = _context.DisciplineChoicePeriods.AsNoTracking().AsQueryable();

        // Фільтрація
        if (queryDto.FacultyId.HasValue)
            query = query.Where(p => p.FacultyId == queryDto.FacultyId.Value);

        if (queryDto.DegreeLevelId.HasValue)
            query = query.Where(p => p.DegreeLevelId == queryDto.DegreeLevelId.Value);

        if (queryDto.PeriodType.HasValue)
            query = query.Where(p => p.PeriodType == queryDto.PeriodType.Value);

        if (queryDto.IsClose.HasValue)
            query = query.Where(p => p.IsClose == queryDto.IsClose.Value);

        if (queryDto.PeriodCourse.HasValue)
            query = query.Where(p => p.PeriodCourse == queryDto.PeriodCourse.Value);

        // Сортування та магічна проекція (ProjectTo)
        return await query
            .OrderByDescending(p => p.StartDate)
            .ProjectTo<DisciplineChoicePeriodDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<DisciplineChoicePeriodDto?> GetDtoByIdAsync(int id)
    {
        return await _context.DisciplineChoicePeriods
            .AsNoTracking()
            .Where(p => p.IdDisciplineChoicePeriod == id)
            .ProjectTo<DisciplineChoicePeriodDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<DisciplineChoicePeriod?> GetEntityByIdAsync(int id)
    {
        return await _context.DisciplineChoicePeriods.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.DisciplineChoicePeriods.AnyAsync(p => p.IdDisciplineChoicePeriod == id);
    }

    public async Task AddAsync(DisciplineChoicePeriod period)
    {
        await _context.DisciplineChoicePeriods.AddAsync(period);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}