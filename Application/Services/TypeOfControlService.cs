using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface ITypeOfControlService
{
    Task<IEnumerable<TypeOfControlDto>> GetAllAsync();
    Task<TypeOfControlDto?> GetByIdAsync(Guid id);
    Task<TypeOfControlDto> CreateAsync(CreateTypeOfControlDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(Guid id, UpdateTypeOfControlDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(Guid id);
}

public class TypeOfControlService : ITypeOfControlService
{
    private readonly ITypeOfControlRepository _repository;
    private readonly IMapper _mapper;

    public TypeOfControlService(ITypeOfControlRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TypeOfControlDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<TypeOfControlDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<TypeOfControlDto> CreateAsync(CreateTypeOfControlDto dto)
    {
        var entity = _mapper.Map<TypeOfControl>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<TypeOfControlDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(Guid id, UpdateTypeOfControlDto dto)
    {
        if (id != dto.IdTypeOfControl)
            return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

        var entity = await _repository.GetEntityByIdAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Type of control not found.");

        _mapper.Map(dto, entity);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Type of control not found.");
            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(Guid id)
    {
        var deletedRows = await _repository.DeleteAsync(id);
        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Type of control not found.");
        return (true, StatusCodes.Status204NoContent, null);
    }
}
