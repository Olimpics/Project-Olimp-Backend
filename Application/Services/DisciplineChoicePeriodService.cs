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
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateDisciplineChoicePeriodDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAfterStartAsync(int id, UpdateDisciplineChoicePeriodAfterStartDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> OpenOrCloseAsync(int id, UpdateDisciplineChoicePeriodOpenOrCloseDto dto);
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
        var period = _mapper.Map<DisciplineChoicePeriod>(dto);

        await _repository.AddAsync(period);
        await _repository.SaveChangesAsync();

        // Витягуємо новостворену DTO (або можеш просто змапити period, якщо там немає складних JOIN'ів)
        return _mapper.Map<DisciplineChoicePeriodDto>(period);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateDisciplineChoicePeriodDto dto)
    {
        if (id != dto.Id) return (false, StatusCodes.Status400BadRequest, "ID mismatch");

        var period = await _repository.GetEntityByIdAsync(id);
        if (period == null) return (false, StatusCodes.Status404NotFound, "Period not found");

        _mapper.Map(dto, period);
        return await SaveWithConcurrencyCheckAsync(id);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAfterStartAsync(int id, UpdateDisciplineChoicePeriodAfterStartDto dto)
    {
        if (id != dto.Id) return (false, StatusCodes.Status400BadRequest, "ID mismatch");

        var period = await _repository.GetEntityByIdAsync(id);
        if (period == null) return (false, StatusCodes.Status404NotFound, "Period not found");

        _mapper.Map(dto, period);
        return await SaveWithConcurrencyCheckAsync(id);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> OpenOrCloseAsync(int id, UpdateDisciplineChoicePeriodOpenOrCloseDto dto)
    {
        if (id != dto.Id) return (false, StatusCodes.Status400BadRequest, "ID mismatch");

        var period = await _repository.GetEntityByIdAsync(id);
        if (period == null) return (false, StatusCodes.Status404NotFound, "Period not found");

        _mapper.Map(dto, period);
        return await SaveWithConcurrencyCheckAsync(id);
    }

    // Допоміжний метод для обробки помилок конкурентності (щоб не дублювати try-catch 3 рази)
    private async Task<(bool success, int statusCode, string? errorMessage)> SaveWithConcurrencyCheckAsync(int id)
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