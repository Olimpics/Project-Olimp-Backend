using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface ICatalogYearRepository
{
    Task<IEnumerable<CatalogYearDto>> GetAllDtoAsync();
    Task<CatalogYearDto?> GetDtoByIdAsync(int id);
    Task<CatalogYear?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(CatalogYear entity);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class CatalogYearRepository : ICatalogYearRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CatalogYearRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatalogYearDto>> GetAllDtoAsync()
    {
        return await _context.CatalogYears
            .AsNoTracking()
            .ProjectTo<CatalogYearDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CatalogYearDto?> GetDtoByIdAsync(int id)
    {
        return await _context.CatalogYears
            .AsNoTracking()
            .Where(c => c.IdCatalogYear == id)
            .ProjectTo<CatalogYearDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<CatalogYear?> GetEntityByIdAsync(int id)
    {
        return await _context.CatalogYears.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.CatalogYears.AnyAsync(c => c.IdCatalogYear == id);
    }

    public async Task AddAsync(CatalogYear entity)
    {
        await _context.CatalogYears.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _context.CatalogYears
            .Where(c => c.IdCatalogYear == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
