using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IMainDisciplineRepository
{
    Task<MainDisciplineDto?> GetDtoByIdAsync(int id);
    Task<MainDiscipline?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(MainDiscipline entity);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class MainDisciplineRepository : IMainDisciplineRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public MainDisciplineRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MainDisciplineDto?> GetDtoByIdAsync(int id)
    {
        return await _context.MainDisciplines
            .AsNoTracking()
            .Where(bmd => bmd.IdBindMainDisciplines == id)
            .ProjectTo<MainDisciplineDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<MainDiscipline?> GetEntityByIdAsync(int id)
    {
        return await _context.MainDisciplines.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.MainDisciplines.AnyAsync(e => e.IdBindMainDisciplines == id);
    }

    public async Task AddAsync(MainDiscipline entity)
    {
        await _context.MainDisciplines.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _context.MainDisciplines
            .Where(bmd => bmd.IdBindMainDisciplines == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
