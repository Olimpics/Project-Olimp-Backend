using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IBindStudentsFavouriteDisciplinesRepository
{
    Task<IEnumerable<AddDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId);
}

public class BindStudentsFavouriteDisciplinesRepository : IBindStudentsFavouriteDisciplinesRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper; // 1. Додаємо мапер

    public BindStudentsFavouriteDisciplinesRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AddDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId)
    {
        return await _context.BindStudentsFavouriteDisciplines
            .AsNoTracking()
            .Where(s => s.StudentId == studentId) // Зверни увагу: тут назва поля з твоєї моделі (StudentId або IdStudent)

            // 2. Магія EF Core: кажемо "візьми сутність AddDiscipline, яка прив'язана до цього запису"
            .Select(s => s.AddDiscipline)

            // 3. Блискавичний мапінг: SQL сам витягне тільки ті колонки, які є в AddDisciplineDto
            .ProjectTo<AddDisciplineDto>(_mapper.ConfigurationProvider)

            .ToListAsync();
    }
}