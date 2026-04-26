using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface ICatalogYearService
{
    Task<IEnumerable<CatalogYearDto>> GetAllAsync();
    Task<CatalogYearDto?> GetByIdAsync(int id);
    Task<CatalogYearDto> CreateAsync(CreateCatalogYearDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateCatalogYearDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class CatalogYearService : ICatalogYearService
{
    private readonly ICatalogYearRepository _repository;
    private readonly IMapper _mapper;

    public CatalogYearService(ICatalogYearRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatalogYearDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<CatalogYearDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<CatalogYearDto> CreateAsync(CreateCatalogYearDto dto)
    {
        var entity = _mapper.Map<CatalogYear>(dto);

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var resultDto = await _repository.GetDtoByIdAsync(entity.IdCatalogYear.GetValueOrDefault());
        return resultDto ?? _mapper.Map<CatalogYearDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateCatalogYearDto dto)
    {
        if (id != dto.IdCatalogYear)
            return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

        var entity = await _repository.GetEntityByIdAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Catalog year not found.");

        _mapper.Map(dto, entity);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Catalog year not found.");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Catalog year not found.");

        return (true, StatusCodes.Status204NoContent, null);
    }
}
