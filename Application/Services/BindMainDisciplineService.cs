using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IBindMainDisciplineService
{
    Task<BindMainDisciplineDto?> GetByIdAsync(int id);
    Task<BindMainDisciplineDto> CreateAsync(CreateBindMainDisciplineDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateBindMainDisciplineDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class BindMainDisciplineService : IBindMainDisciplineService
{
    private readonly IBindMainDisciplineRepository _repository;
    private readonly IMapper _mapper;

    public BindMainDisciplineService(IBindMainDisciplineRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BindMainDisciplineDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<BindMainDisciplineDto> CreateAsync(CreateBindMainDisciplineDto dto)
    {
        var entity = _mapper.Map<BindMainDiscipline>(dto);

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // Повертаємо DTO без повторного походу в БД
        return _mapper.Map<BindMainDisciplineDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateBindMainDisciplineDto dto)
    {
        if (id != dto.IdBindMainDisciplines)
            return (false, StatusCodes.Status400BadRequest, "Route id does not match body id.");

        var entity = await _repository.GetEntityByIdAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        _mapper.Map(dto, entity);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Binding not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);
        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        return (true, StatusCodes.Status204NoContent, null);
    }
}