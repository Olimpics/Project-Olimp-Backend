using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IEducationStatusService
{
    Task<IEnumerable<EducationStatusDto>> GetAllAsync();
    Task<EducationStatusDto?> GetByIdAsync(int id);
    Task<EducationStatusDto> CreateAsync(EducationStatusDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, EducationStatusDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class EducationStatusService : IEducationStatusService
{
    private readonly IEducationStatusRepository _repository;
    private readonly IMapper _mapper;

    public EducationStatusService(IEducationStatusRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EducationStatusDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<EducationStatusDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<EducationStatusDto> CreateAsync(EducationStatusDto dto)
    {
        var status = _mapper.Map<EducationStatus>(dto);

        await _repository.AddAsync(status);
        await _repository.SaveChangesAsync();

        return _mapper.Map<EducationStatusDto>(status);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, EducationStatusDto dto)
    {
        if (id != dto.IdEducationStatus)
            return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

        var status = await _repository.GetEntityByIdAsync(id);
        if (status == null)
            return (false, StatusCodes.Status404NotFound, "Education status not found.");

        _mapper.Map(dto, status);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Education status not found.");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Education status not found.");

        return (true, StatusCodes.Status204NoContent, null);
    }
}