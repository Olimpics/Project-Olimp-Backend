using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IBindStudentsFavouriteDisciplinesRepository
{
    Task<IEnumerable<StudentFavouriteDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId);
    Task<StudentFavouriteDisciplineDto?> GetFavoriteByIdAsync(int id);
    Task<bool> ExistsAsync(int studentId, int disciplineId);
    Task AddAsync(BindStudentsFavouriteDiscipline entity);
    Task<int> DeleteAsync(int bindId);
    Task SaveChangesAsync();
}

public class BindStudentsFavouriteDisciplinesRepository : IBindStudentsFavouriteDisciplinesRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper; // Повертаємо мапер назад!

    public BindStudentsFavouriteDisciplinesRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StudentFavouriteDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId)
    {
        return await _context.BindStudentsFavouriteDisciplines
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId && s.IdAddDisciplineNavigation != null)
            // AutoMapper тепер сам побудує правильний SQL SELECT завдяки правилу в MappingProfile!
            .ProjectTo<StudentFavouriteDisciplineDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // Отримуємо один запис після створення (щоб повернути красиву DTO)
    public async Task<StudentFavouriteDisciplineDto?> GetFavoriteByIdAsync(int id)
    {
        return await _context.BindStudentsFavouriteDisciplines
            .AsNoTracking()
            .Where(b => b.IdBindStudentsFavouriteDisciplines == id && b.IdAddDisciplineNavigation != null)
            .ProjectTo<StudentFavouriteDisciplineDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    // Перевірка на дублікати
    public async Task<bool> ExistsAsync(int studentId, int disciplineId)
    {
        return await _context.BindStudentsFavouriteDisciplines
            .AnyAsync(b => b.IdStudent == studentId && b.IdAddDiscipline == disciplineId);
    }

    public async Task AddAsync(BindStudentsFavouriteDiscipline entity)
    {
        await _context.BindStudentsFavouriteDisciplines.AddAsync(entity);
    }

    // Блискавичне видалення без завантаження в пам'ять
    public async Task<int> DeleteAsync(int bindId)
    {
        return await _context.BindStudentsFavouriteDisciplines
            .Where(b => b.IdBindStudentsFavouriteDisciplines == bindId)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}