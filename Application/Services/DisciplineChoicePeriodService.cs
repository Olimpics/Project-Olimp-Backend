using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IDisciplineChoicePeriodService
{
    Task<List<DisciplineChoicePeriodDto>> GetAllAsync(GetDisciplineChoicePeriodsQueryDto queryDto);
    Task<DisciplineChoicePeriodDto> CreateAsync(CreateDisciplineChoicePeriodDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(Guid id, UpdateDisciplineChoicePeriodDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAfterStartAsync(Guid id, UpdateDisciplineChoicePeriodAfterStartDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> OpenOrCloseAsync(Guid id, UpdateDisciplineChoicePeriodOpenOrCloseDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> ApproveAsync(Guid id);
}

public class DisciplineChoicePeriodService : IDisciplineChoicePeriodService
{
    private readonly IDisciplineChoicePeriodRepository _repository;
    private readonly IMapper _mapper;
    private readonly ISystemEventService _systemEventService;
    private readonly IStudentChoiceCacheService _cacheService;
    private readonly IDisciplineCacheService _disciplineCacheService;
    private readonly Data.AppDbContext _context;

    public DisciplineChoicePeriodService(
        IDisciplineChoicePeriodRepository repository, 
        IMapper mapper, 
        ISystemEventService systemEventService,
        IStudentChoiceCacheService cacheService,
        IDisciplineCacheService disciplineCacheService,
        Data.AppDbContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _systemEventService = systemEventService;
        _cacheService = cacheService;
        _disciplineCacheService = disciplineCacheService;
        _context = context;
    }

    public async Task<List<DisciplineChoicePeriodDto>> GetAllAsync(GetDisciplineChoicePeriodsQueryDto queryDto)
    {
        return await _repository.GetAllDtoAsync(queryDto);
    }

    public async Task<DisciplineChoicePeriodDto> CreateAsync(CreateDisciplineChoicePeriodDto dto)
    {
        if (dto.EndOfCheckPeriod.HasValue && dto.EndDate.HasValue && dto.EndOfCheckPeriod < dto.EndDate)
            throw new ArgumentException("EndOfCheckPeriod cannot be less than EndDate");

        var period = _mapper.Map<DisciplineChoicePeriod>(dto);
        period.IsConfirm = false;

        await _repository.AddAsync(period);
        await _repository.SaveChangesAsync();

        // Create copies for all departments
        var departments = await _context.Departments.Where(d => d.Avail).ToListAsync();
        foreach (var dept in departments)
        {
            if (dept.IdDepartment == period.DepartmentId) continue;

            var copy = new DisciplineChoicePeriod
            {
                DepartmentId = dept.IdDepartment,
                PeriodType = period.PeriodType,
                PeriodCourse = period.PeriodCourse,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                EndOfCheckPeriod = period.EndOfCheckPeriod,
                CatalogYearId = period.CatalogYearId,
                DegreeLevelId = period.DegreeLevelId,
                IsForBothSemester = period.IsForBothSemester,
                SpecialityId = period.SpecialityId,
                IsShort = period.IsShort,
                ParentId = period.IdDisciplineChoicePeriod,
                IsConfirm = false,
                IsClose = period.IsClose
            };
            _context.DisciplineChoicePeriods.Add(copy);
        }
        await _context.SaveChangesAsync();

        await _systemEventService.DispatchEventAsync("Creation of a selection period");

        return _mapper.Map<DisciplineChoicePeriodDto>(period);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> ApproveAsync(Guid id)
    {
        var period = await _context.DisciplineChoicePeriods
            .Include(p => p.CatalogYear)
            .FirstOrDefaultAsync(p => p.IdDisciplineChoicePeriod == id);

        if (period == null) return (false, StatusCodes.Status404NotFound, "Period not found");
        if (period.IsConfirm) return (false, StatusCodes.Status400BadRequest, "Period already confirmed");

        period.IsConfirm = true;
        await _context.SaveChangesAsync();

        // Initialize discipline occupancy cache
        var disciplinesQuery = _context.SelectiveDisciplines
            .Include(d => d.Catalog)
            .Where(d => d.Catalog.YearStart == period.CatalogYear.YearStart);

        if (period.DegreeLevelId != Guid.Empty)
        {
            disciplinesQuery = disciplinesQuery.Where(d => d.DegreeLevelId == period.DegreeLevelId);
        }

        var disciplines = await disciplinesQuery.ToListAsync();
        foreach (var disc in disciplines)
        {
            await _disciplineCacheService.SetOccupancyAsync(disc.IdSelectiveDisciplines, 0);
        }

        // Process students and load to cache
        await _cacheService.InitializeCacheAsync(null, period);

        return (true, StatusCodes.Status200OK, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(Guid id, UpdateDisciplineChoicePeriodDto dto)
    {
        if (id != dto.Id) return (false, StatusCodes.Status400BadRequest, "ID mismatch");
        if (dto.EndOfCheckPeriod.HasValue && dto.EndDate.HasValue && dto.EndOfCheckPeriod < dto.EndDate)
            return (false, StatusCodes.Status400BadRequest, "EndOfCheckPeriod cannot be less than EndDate");

        var period = await _repository.GetEntityByIdAsync(id);
        if (period == null) return (false, StatusCodes.Status404NotFound, "Period not found");

        _mapper.Map(dto, period);
        // Ensure IsClose is not changed by mapper if DTO doesn't have it (or manually reset if it does)
        return await SaveWithConcurrencyCheckAsync(id);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAfterStartAsync(Guid id, UpdateDisciplineChoicePeriodAfterStartDto dto)
    {
        if (id != dto.Id) return (false, StatusCodes.Status400BadRequest, "ID mismatch");
        if (dto.EndOfCheckPeriod.HasValue && dto.EndDate.HasValue && dto.EndOfCheckPeriod < dto.EndDate)
            return (false, StatusCodes.Status400BadRequest, "EndOfCheckPeriod cannot be less than EndDate");

        var period = await _repository.GetEntityByIdAsync(id);
        if (period == null) return (false, StatusCodes.Status404NotFound, "Period not found");

        _mapper.Map(dto, period);
        return await SaveWithConcurrencyCheckAsync(id);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> OpenOrCloseAsync(Guid id, UpdateDisciplineChoicePeriodOpenOrCloseDto dto)
    {
        var period = await _context.DisciplineChoicePeriods.FindAsync(id);
        if (period == null) return (false, StatusCodes.Status404NotFound, "Period not found");

        period.IsClose = dto.IsClose;
        await _context.SaveChangesAsync();

        if (period.IsClose)
        {
            await _cacheService.ClearCacheForStudentsInPeriodAsync(period);
        }
        else
        {
            await _cacheService.InitializeCacheAsync(null, period);
        }

        return (true, StatusCodes.Status204NoContent, null);
    }


    // ��������� ����� ��� ������� ������� ������������� (��� �� ��������� try-catch 3 ����)
    private async Task<(bool success, int statusCode, string? errorMessage)> SaveWithConcurrencyCheckAsync(Guid id)
    {
        try
        {
            await _repository.SaveChangesAsync();
            return (true, StatusCodes.Status204NoContent, null);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Period not found");
            throw;
        }
    }
}