using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;

namespace OlimpBack.Application.Services;

public class FacultyService : IFacultyService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FacultyService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FacultyDto>> GetFacultiesAsync()
    {
        var faculties = await _context.Faculties.ToListAsync();
        return _mapper.Map<IEnumerable<FacultyDto>>(faculties);
    }

    public async Task<FacultyDto?> GetFacultyAsync(int id)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null)
            return null;

        return _mapper.Map<FacultyDto>(faculty);
    }

    public async Task<FacultyDto> CreateFacultyAsync(FacultyCreateDto dto)
    {
        var faculty = _mapper.Map<Models.Faculty>(dto);
        _context.Faculties.Add(faculty);
        await _context.SaveChangesAsync();

        return _mapper.Map<FacultyDto>(faculty);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateFacultyAsync(int id, FacultyDto dto)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null)
            return (false, StatusCodes.Status404NotFound, "Faculty not found");

        _mapper.Map(dto, faculty);
        await _context.SaveChangesAsync();

        return (true, StatusCodes.Status204NoContent, null);
    }
}

