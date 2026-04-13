using AutoMapper;
using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class EducationalProgramService : IEducationalProgramService
{
    private readonly IEducationalProgramRepository _repository;
    private readonly IMapper _mapper;

    public EducationalProgramService(IEducationalProgramRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<List<EducationalProgramFilterDto>> GetEducationalProgramsForFilterAsync(string? search) =>
        _repository.GetForFilterAsync(search);

    public async Task<PaginatedResponseDto<EducationalProgramDto>> GetEducationalProgramsAsync(EducationalProgramListQueryDto queryDto)
    {
        var (totalCount, items) = await _repository.GetPagedAsync(queryDto);
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        return new PaginatedResponseDto<EducationalProgramDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items
        };
    }

    public async Task<EducationalProgramDto?> GetEducationalProgramAsync(int id) =>
        await _repository.GetDtoByIdAsync(id);

    public async Task<EducationalProgramDto> CreateEducationalProgramAsync(CreateEducationalProgramDto dto)
    {
        var program = _mapper.Map<EducationalProgram>(dto);

        await _repository.AddAsync(program);
        await _repository.SaveChangesAsync();

        var result = _mapper.Map<EducationalProgramDto>(program);
        result.StudentsCount = 0;
        result.DisciplinesCount = 0;

        return result;
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateEducationalProgramAsync(int id, UpdateEducationalProgramDto dto)
    {
        var program = await _repository.GetEntityByIdAsync(id);
        if (program == null)
            return (false, StatusCodes.Status404NotFound, "Educational program not found");

        _mapper.Map(dto, program);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Educational program not found");
            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteEducationalProgramAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);
        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Educational program not found");

        return (true, StatusCodes.Status204NoContent, null);
    }
}