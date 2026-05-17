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
}

public class DisciplineChoicePeriodService : IDisciplineChoicePeriodService
{
    private readonly IDisciplineChoicePeriodRepository _repository;
    private readonly IMapper _mapper;

    public DisciplineChoicePeriodService(IDisciplineChoicePeriodRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
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

        await _repository.AddAsync(period);
        await _repository.SaveChangesAsync();

        return _mapper.Map<DisciplineChoicePeriodDto>(period);
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
        // This method is now effectively a no-op for closing, but we'll keep the signature
        return (true, StatusCodes.Status204NoContent, "Manual closing is disabled");
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