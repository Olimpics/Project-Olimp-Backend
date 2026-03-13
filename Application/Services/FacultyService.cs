using AutoMapper;
using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class FacultyService : IFacultyService
{
    private readonly IFacultyRepository _repository;
    private readonly IMapper _mapper;

    public FacultyService(IFacultyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FacultyDto>> GetFacultiesAsync() =>
        await _repository.GetAllDtoAsync();

    public async Task<FacultyDto?> GetFacultyAsync(int id) =>
        await _repository.GetDtoByIdAsync(id);

    public async Task<FacultyDto> CreateFacultyAsync(FacultyCreateDto dto)
    {
        var faculty = _mapper.Map<Faculty>(dto);
        await _repository.AddAsync(faculty);
        await _repository.SaveChangesAsync();

        return _mapper.Map<FacultyDto>(faculty);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateFacultyAsync(int id, FacultyDto dto)
    {
        var faculty = await _repository.GetEntityByIdAsync(id);
        if (faculty == null)
            return (false, StatusCodes.Status404NotFound, "Faculty not found");

        _mapper.Map(dto, faculty);
        await _repository.SaveChangesAsync();

        return (true, StatusCodes.Status204NoContent, null);
    }
}