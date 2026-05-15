using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Infrastructure.Database.Repositories;

public interface INormativeRepository
{
    Task<IEnumerable<NormativeDto>> GetAllDtoAsync();
    Task<NormativeDto?> GetDtoByIdAsync(Guid id);
    Task<Normative?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(Normative entity);
    Task<int> DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public class NormativeRepository : INormativeRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public NormativeRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NormativeDto>> GetAllDtoAsync()
    {
        return await _context.Normatives
            .AsNoTracking()
            .ProjectTo<NormativeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<NormativeDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.Normatives
            .AsNoTracking()
            .Where(n => n.IdNormative == id)
            .ProjectTo<NormativeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<Normative?> GetEntityByIdAsync(Guid id)
    {
        return await _context.Normatives.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Normatives.AnyAsync(n => n.IdNormative == id);
    }

    public async Task AddAsync(Normative entity)
    {
        await _context.Normatives.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        return await _context.Normatives
            .Where(n => n.IdNormative == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}