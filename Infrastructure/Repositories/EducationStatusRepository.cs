using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IEducationStatusRepository
{
    Task<IEnumerable<EducationStatusDto>> GetAllDtoAsync();
    Task<EducationStatusDto?> GetDtoByIdAsync(int id);
    Task<EducationStatus?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(EducationStatus status);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class EducationStatusRepository : IEducationStatusRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EducationStatusRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EducationStatusDto>> GetAllDtoAsync()
    {
        // SQL одразу повертає потрібні поля DTO, оминаючи завантаження важких сутностей
        return await _context.EducationStatuses
            .AsNoTracking()
            .ProjectTo<EducationStatusDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EducationStatusDto?> GetDtoByIdAsync(int id)
    {
        return await _context.EducationStatuses
            .AsNoTracking()
            .Where(es => es.IdEducationStatus == id)
            .ProjectTo<EducationStatusDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<EducationStatus?> GetEntityByIdAsync(int id)
    {
        return await _context.EducationStatuses.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.EducationStatuses.AnyAsync(es => es.IdEducationStatus == id);
    }

    public async Task AddAsync(EducationStatus status)
    {
        await _context.EducationStatuses.AddAsync(status);
    }

    public async Task<int> DeleteAsync(int id)
    {
        // Видалення одним SQL-запитом без FindAsync
        return await _context.EducationStatuses
            .Where(es => es.IdEducationStatus == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}