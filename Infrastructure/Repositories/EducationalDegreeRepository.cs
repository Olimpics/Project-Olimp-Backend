using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IEducationalDegreeRepository
{
    Task<IEnumerable<EducationalDegreeDto>> GetAllDtoAsync();
    Task<EducationalDegreeDto?> GetDtoByIdAsync(int id);
    Task<EducationalDegree?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(EducationalDegree degree);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class EducationalDegreeRepository : IEducationalDegreeRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EducationalDegreeRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EducationalDegreeDto>> GetAllDtoAsync()
    {
        // Швидка проекція сутностей в DTO прямо в SQL
        return await _context.EducationalDegrees
            .AsNoTracking()
            .ProjectTo<EducationalDegreeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EducationalDegreeDto?> GetDtoByIdAsync(int id)
    {
        return await _context.EducationalDegrees
            .AsNoTracking()
            .Where(ed => ed.IdEducationalDegree == id)
            .ProjectTo<EducationalDegreeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<EducationalDegree?> GetEntityByIdAsync(int id)
    {
        // Відстеження (Tracking) зберігається, оскільки ми будемо оновлювати цю сутність
        return await _context.EducationalDegrees.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.EducationalDegrees.AnyAsync(ed => ed.IdEducationalDegree == id);
    }

    public async Task AddAsync(EducationalDegree degree)
    {
        await _context.EducationalDegrees.AddAsync(degree);
    }

    public async Task<int> DeleteAsync(int id)
    {
        // Сучасне видалення в один SQL-запит
        return await _context.EducationalDegrees
            .Where(ed => ed.IdEducationalDegree == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}