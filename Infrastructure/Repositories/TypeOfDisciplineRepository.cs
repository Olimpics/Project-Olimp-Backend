using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface ITypeOfDisciplineRepository
{
    Task<IEnumerable<TypeOfDisciplineDto>> GetAllDtoAsync();
    Task<TypeOfDisciplineDto?> GetDtoByIdAsync(int id);
    Task<TypeOfDiscipline?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(TypeOfDiscipline entity);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class TypeOfDisciplineRepository : ITypeOfDisciplineRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public TypeOfDisciplineRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<TypeOfDisciplineDto>> GetAllDtoAsync()
    {
        return await _context.TypeOfDisciplines
            .AsNoTracking()
            .ProjectTo<TypeOfDisciplineDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
    public async Task<TypeOfDisciplineDto?> GetDtoByIdAsync(int id)
    {
        return await _context.TypeOfDisciplines
            .AsNoTracking()
            .Where(t => t.IdTypeOfDiscipline == id)
            .ProjectTo<TypeOfDisciplineDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }
    public async Task<TypeOfDiscipline?> GetEntityByIdAsync(int id)
    {
        return await _context.TypeOfDisciplines.FindAsync(id);
    }
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.TypeOfDisciplines.AnyAsync(t => t.IdTypeOfDiscipline == id);
    }
    public async Task AddAsync(TypeOfDiscipline entity)
    {
        await _context.TypeOfDisciplines.AddAsync(entity);
    }
    public async Task<int> DeleteAsync(int id)
    {
        var entity = await GetEntityByIdAsync(id);
        if (entity == null) return 0;
        _context.TypeOfDisciplines.Remove(entity);
        return 1;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}