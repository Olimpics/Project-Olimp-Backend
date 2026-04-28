using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IMainDisciplineService
{
    Task<MainDisciplineDto?> GetByIdAsync(int id);
    Task<MainDisciplineDto> CreateAsync(CreateMainDisciplineDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateMainDisciplineDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class MainDisciplineService : IMainDisciplineService
{
    private readonly IMainDisciplineRepository _repository;
    private readonly IMapper _mapper;

    public MainDisciplineService(IMainDisciplineRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<MainDisciplineDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<MainDisciplineDto> CreateAsync(CreateMainDisciplineDto dto)
    {
        var entity = _mapper.Map<MainDiscipline>(dto);

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<MainDisciplineDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateMainDisciplineDto dto)
    {
        if (id != dto.IdMainDisciplines)
            return (false, StatusCodes.Status400BadRequest, "Route id does not match body id.");

        var entity = await _repository.GetEntityByIdAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Discipline not found");

        _mapper.Map(dto, entity);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Discipline not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);
        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Discipline not found");

        return (true, StatusCodes.Status204NoContent, null);
    }
}
