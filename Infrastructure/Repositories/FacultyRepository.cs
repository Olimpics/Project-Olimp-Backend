using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IFacultyRepository
{
    Task<IEnumerable<FacultyDto>> GetAllDtoAsync();
    Task<FacultyDto?> GetDtoByIdAsync(int id);
    Task<Faculty?> GetEntityByIdAsync(int id);
    Task AddAsync(Faculty faculty);
    Task SaveChangesAsync();
}

public class FacultyRepository : IFacultyRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    // Передаємо IMapper у репозиторій, щоб використовувати ProjectTo прямо в запитах до БД
    public FacultyRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FacultyDto>> GetAllDtoAsync()
    {
        return await _context.Faculties
            .AsNoTracking()
            .ProjectTo<FacultyDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<FacultyDto?> GetDtoByIdAsync(int id)
    {
        return await _context.Faculties
            .AsNoTracking()
            .Where(f => f.IdFaculty == id)
            .ProjectTo<FacultyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<Faculty?> GetEntityByIdAsync(int id)
    {
        // Для Update нам потрібна сутність з відстеженням (Tracking)
        return await _context.Faculties.FindAsync(id);
    }

    public async Task AddAsync(Faculty faculty)
    {
        await _context.Faculties.AddAsync(faculty);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}