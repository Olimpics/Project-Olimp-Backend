using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface ICatalogYearSelectiveService
{
    Task<IEnumerable<CatalogYearSelectiveDto>> GetAllAsync();
    Task<CatalogYearSelectiveDto?> GetByIdAsync(int id);
    Task<CatalogYearSelectiveDto> CreateAsync(CreateCatalogYearSelectiveDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateCatalogYearSelectiveDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class CatalogYearSelectiveService : ICatalogYearSelectiveService
{
    private readonly ICatalogYearSelectiveRepository _repository;
    private readonly IMapper _mapper;

    public CatalogYearSelectiveService(ICatalogYearSelectiveRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatalogYearSelectiveDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<CatalogYearSelectiveDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<CatalogYearSelectiveDto> CreateAsync(CreateCatalogYearSelectiveDto dto)
    {
        var entity = _mapper.Map<CatalogYearsSelective>(dto);

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var resultDto = await _repository.GetDtoByIdAsync(entity.IdCatalogYear);
        return resultDto ?? _mapper.Map<CatalogYearSelectiveDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateCatalogYearSelectiveDto dto)
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
