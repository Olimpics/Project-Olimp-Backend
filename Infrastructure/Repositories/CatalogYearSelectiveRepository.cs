using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface ICatalogYearSelectiveRepository
{
    Task<IEnumerable<CatalogYearSelectiveDto>> GetAllDtoAsync();
    Task<CatalogYearSelectiveDto?> GetDtoByIdAsync(int id);
    Task<CatalogYearsSelective?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(CatalogYearsSelective entity);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class CatalogYearSelectiveRepository : ICatalogYearSelectiveRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CatalogYearSelectiveRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatalogYearSelectiveDto>> GetAllDtoAsync()
    {
        return await _context.CatalogYearsSelectives
            .AsNoTracking()
            .ProjectTo<CatalogYearSelectiveDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CatalogYearSelectiveDto?> GetDtoByIdAsync(int id)
    {
        return await _context.CatalogYearsSelectives
            .AsNoTracking()
            .Where(c => c.IdCatalogYear == id)
            .ProjectTo<CatalogYearSelectiveDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<CatalogYearsSelective?> GetEntityByIdAsync(int id)
    {
        return await _context.CatalogYearsSelectives.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.CatalogYearsSelectives.AnyAsync(c => c.IdCatalogYear == id);
    }

    public async Task AddAsync(CatalogYearsSelective entity)
    {
        await _context.CatalogYearsSelectives.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _context.CatalogYearsSelectives
            .Where(c => c.IdCatalogYear == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
