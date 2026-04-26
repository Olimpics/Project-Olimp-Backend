using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface INormativeService
{
    Task<IEnumerable<NormativeDto>> GetAllAsync();
    Task<NormativeDto?> GetByIdAsync(int id);
    Task<NormativeDto> CreateAsync(CreateNormativeDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateNormativeDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class NormativeService : INormativeService
{
    private readonly INormativeRepository _repository;
    private readonly IMapper _mapper;

    public NormativeService(INormativeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NormativeDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<NormativeDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<NormativeDto> CreateAsync(CreateNormativeDto dto)
    {
        var entity = _mapper.Map<Normative>(dto);

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // Повертаємо повноцінну DTO з БД (якщо треба підтягнути назви через зв'язки)
        var resultDto = await _repository.GetDtoByIdAsync(entity.IdNormative.GetValueOrDefault());
        return resultDto ?? _mapper.Map<NormativeDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateNormativeDto dto)
    {
        if (id != dto.IdNormative)
            return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

        var entity = await _repository.GetEntityByIdAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Normative not found.");

        _mapper.Map(dto, entity);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Normative not found.");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Normative not found.");

        return (true, StatusCodes.Status204NoContent, null);
    }
}