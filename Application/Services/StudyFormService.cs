using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IStudyFormService
{
    Task<IEnumerable<StudyFormDto>> GetAllAsync();
    Task<StudyFormDto?> GetByIdAsync(int id);
    Task<StudyFormDto> CreateAsync(StudyFormDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, StudyFormDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class StudyFormService : IStudyFormService
{
    private readonly IStudyFormRepository _repository;
    private readonly IMapper _mapper;

    public StudyFormService(IStudyFormRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StudyFormDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<StudyFormDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<StudyFormDto> CreateAsync(StudyFormDto dto)
    {
        var form = _mapper.Map<StudyForm>(dto);

        await _repository.AddAsync(form);
        await _repository.SaveChangesAsync();

        // Повертаємо мапінг створеної сутності
        return _mapper.Map<StudyFormDto>(form);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, StudyFormDto dto)
    {
        if (id != dto.IdStudyForm)
            return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

        var form = await _repository.GetEntityByIdAsync(id);
        if (form == null)
            return (false, StatusCodes.Status404NotFound, "Study form not found.");

        _mapper.Map(dto, form);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Study form not found.");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Study form not found.");

        return (true, StatusCodes.Status204NoContent, null);
    }
}