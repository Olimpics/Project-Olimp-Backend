using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface ICatalogYearMainService
{
    Task<IEnumerable<CatalogYearMainDto>> GetAllAsync();
    Task<CatalogYearMainDto?> GetByIdAsync(int id);
    Task<CatalogYearMainDto> CreateAsync(CreateCatalogYearMainDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateCatalogYearMainDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class CatalogYearMainService : ICatalogYearMainService
{
    private readonly ICatalogYearMainRepository _repository;
    private readonly IMapper _mapper;

    public CatalogYearMainService(ICatalogYearMainRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatalogYearMainDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<CatalogYearMainDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<CatalogYearMainDto> CreateAsync(CreateCatalogYearMainDto dto)
    {
        var entity = _mapper.Map<CatalogYearsMain>(dto);

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var resultDto = await _repository.GetDtoByIdAsync(entity.IdCatalogYear);
        return resultDto ?? _mapper.Map<CatalogYearMainDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateCatalogYearMainDto dto)
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
