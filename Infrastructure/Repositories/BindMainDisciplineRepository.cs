using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IBindMainDisciplineRepository
{
    Task<BindMainDisciplineDto?> GetDtoByIdAsync(int id);
    Task<BindMainDiscipline?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(BindMainDiscipline entity);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class BindMainDisciplineRepository : IBindMainDisciplineRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BindMainDisciplineRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BindMainDisciplineDto?> GetDtoByIdAsync(int id)
    {
        // Проекція замість Include! База витягне лише те, що потрібно для DTO
        return await _context.BindMainDisciplines
            .AsNoTracking()
            .Where(bmd => bmd.IdBindMainDisciplines == id)
            .ProjectTo<BindMainDisciplineDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<BindMainDiscipline?> GetEntityByIdAsync(int id)
    {
        return await _context.BindMainDisciplines.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.BindMainDisciplines.AnyAsync(e => e.IdBindMainDisciplines == id);
    }

    public async Task AddAsync(BindMainDiscipline entity)
    {
        await _context.BindMainDisciplines.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        // Швидке видалення без попереднього завантаження в пам'ять
        return await _context.BindMainDisciplines
            .Where(bmd => bmd.IdBindMainDisciplines == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}