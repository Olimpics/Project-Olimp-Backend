using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IEducationStatusRepository
{
    Task<IEnumerable<EducationStatusDto>> GetAllDtoAsync();
    Task<EducationStatusDto?> GetDtoByIdAsync(Guid id);
    Task<EducationStatus?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(EducationStatus status);
    Task<int> DeleteAsync(Guid id);
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

    public async Task<EducationStatusDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.EducationStatuses
            .AsNoTracking()
            .Where(es => es.IdEducationStatus == id)
            .ProjectTo<EducationStatusDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<EducationStatus?> GetEntityByIdAsync(Guid id)
    {
        return await _context.EducationStatuses.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.EducationStatuses.AnyAsync(es => es.IdEducationStatus == id);
    }

    public async Task AddAsync(EducationStatus status)
    {
        await _context.EducationStatuses.AddAsync(status);
    }

    public async Task<int> DeleteAsync(Guid id)
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