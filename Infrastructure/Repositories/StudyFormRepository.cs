using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IStudyFormRepository
{
    Task<IEnumerable<StudyFormDto>> GetAllDtoAsync();
    Task<StudyFormDto?> GetDtoByIdAsync(Guid id);
    Task<StudyForm?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(StudyForm form);
    Task<int> DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public class StudyFormRepository : IStudyFormRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudyFormRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StudyFormDto>> GetAllDtoAsync()
    {
        // ProjectTo конвертує сутності в DTO на рівні SQL-запиту
        return await _context.StudyForms
            .AsNoTracking()
            .ProjectTo<StudyFormDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<StudyFormDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.StudyForms
            .AsNoTracking()
            .Where(sf => sf.IdStudyForm == id)
            .ProjectTo<StudyFormDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<StudyForm?> GetEntityByIdAsync(Guid id)
    {
        // FindAsync потрібен для методу Update, де ми змінюємо існуючу сутність
        return await _context.StudyForms.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.StudyForms.AnyAsync(sf => sf.IdStudyForm == id);
    }

    public async Task AddAsync(StudyForm form)
    {
        await _context.StudyForms.AddAsync(form);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        // Сучасне швидке видалення
        return await _context.StudyForms
            .Where(sf => sf.IdStudyForm == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}