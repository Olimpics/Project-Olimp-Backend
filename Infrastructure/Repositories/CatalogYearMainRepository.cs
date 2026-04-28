using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface ICatalogYearMainRepository
{
    Task<IEnumerable<CatalogYearMainDto>> GetAllDtoAsync();
    Task<CatalogYearMainDto?> GetDtoByIdAsync(int id);
    Task<CatalogYearsMain?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(CatalogYearsMain entity);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class CatalogYearMainRepository : ICatalogYearMainRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CatalogYearMainRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatalogYearMainDto>> GetAllDtoAsync()
    {
        return await _context.CatalogYearsMains
            .AsNoTracking()
            .ProjectTo<CatalogYearMainDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CatalogYearMainDto?> GetDtoByIdAsync(int id)
    {
        return await _context.CatalogYearsMains
            .AsNoTracking()
            .Where(c => c.IdCatalogYear == id)
            .ProjectTo<CatalogYearMainDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<CatalogYearsMain?> GetEntityByIdAsync(int id)
    {
        return await _context.CatalogYearsMains.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.CatalogYearsMains.AnyAsync(c => c.IdCatalogYear == id);
    }

    public async Task AddAsync(CatalogYearsMain entity)
    {
        await _context.CatalogYearsMains.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _context.CatalogYearsMains
            .Where(c => c.IdCatalogYear == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
