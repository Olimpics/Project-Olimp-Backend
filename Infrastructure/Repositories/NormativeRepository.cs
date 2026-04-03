using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface INormativeRepository
{
    Task<IEnumerable<NormativeDto>> GetAllDtoAsync();
    Task<NormativeDto?> GetDtoByIdAsync(int id);
    Task<Normative?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Normative entity);
    Task<int> DeleteAsync(int id);
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

    public async Task<NormativeDto?> GetDtoByIdAsync(int id)
    {
        return await _context.Normatives
            .AsNoTracking()
            .Where(n => n.IdNormative == id)
            .ProjectTo<NormativeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<Normative?> GetEntityByIdAsync(int id)
    {
        return await _context.Normatives.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Normatives.AnyAsync(n => n.IdNormative == id);
    }

    public async Task AddAsync(Normative entity)
    {
        await _context.Normatives.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
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