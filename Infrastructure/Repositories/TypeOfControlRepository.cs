using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface ITypeOfControlRepository
{
    Task<IEnumerable<TypeOfControlDto>> GetAllDtoAsync();
    Task<TypeOfControlDto?> GetDtoByIdAsync(Guid id);
    Task<TypeOfControl?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(TypeOfControl entity);
    Task<int> DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public class TypeOfControlRepository : ITypeOfControlRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TypeOfControlRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TypeOfControlDto>> GetAllDtoAsync()
    {
        return await _context.TypeOfControls
            .AsNoTracking()
            .ProjectTo<TypeOfControlDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TypeOfControlDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.TypeOfControls
            .AsNoTracking()
            .Where(t => t.IdTypeOfControl == id)
            .ProjectTo<TypeOfControlDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<TypeOfControl?> GetEntityByIdAsync(Guid id)
    {
        return await _context.TypeOfControls.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.TypeOfControls.AnyAsync(t => t.IdTypeOfControl == id);
    }

    public async Task AddAsync(TypeOfControl entity)
    {
        await _context.TypeOfControls.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        return await _context.TypeOfControls
            .Where(t => t.IdTypeOfControl == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
