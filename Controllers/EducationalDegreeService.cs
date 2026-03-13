using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IEducationalDegreeService
{
    Task<IEnumerable<EducationalDegreeDto>> GetAllAsync();
    Task<EducationalDegreeDto?> GetByIdAsync(int id);
    Task<EducationalDegreeDto> CreateAsync(CreateEducationalDegreeDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateEducationalDegreeDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class EducationalDegreeService : IEducationalDegreeService
{
    private readonly IEducationalDegreeRepository _repository;
    private readonly IMapper _mapper;

    public EducationalDegreeService(IEducationalDegreeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EducationalDegreeDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<EducationalDegreeDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<EducationalDegreeDto> CreateAsync(CreateEducationalDegreeDto dto)
    {
        var degree = _mapper.Map<EducationalDegree>(dto);

        await _repository.AddAsync(degree);
        await _repository.SaveChangesAsync();

        return _mapper.Map<EducationalDegreeDto>(degree);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateEducationalDegreeDto dto)
    {
        if (id != dto.IdEducationalDegree)
            return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

        var degree = await _repository.GetEntityByIdAsync(id);
        if (degree == null)
            return (false, StatusCodes.Status404NotFound, "Educational degree not found.");

        _mapper.Map(dto, degree);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Educational degree not found.");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Educational degree not found.");

        return (true, StatusCodes.Status204NoContent, null);
    }
}