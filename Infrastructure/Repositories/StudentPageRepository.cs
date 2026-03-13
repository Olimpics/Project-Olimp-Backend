using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IStudentPageRepository
{
    Task<StudentDisciplinesDto?> GetStudentDisciplinesAsync(int studentId);
}

public class StudentPageRepository : IStudentPageRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentPageRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<StudentDisciplinesDto?> GetStudentDisciplinesAsync(int studentId)
    {
        // 1. Блискавична вибірка через Select БЕЗ Include!
        var data = await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId)
            .Select(s => new
            {
                s.IdStudent,
                s.NameStudent,
                // EF Core сам зробить JOIN і витягне тільки пов'язані колекції
                MainDisciplines = s.EducationalProgram != null ? s.EducationalProgram.BindMainDisciplines : null,
                AdditionalDisciplines = s.BindAddDisciplines
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return null;

        // 2. Формуємо DTO в пам'яті (безпечно і швидко)
        return new StudentDisciplinesDto
        {
            StudentId = data.IdStudent,
            StudentName = data.NameStudent ?? "",
            MainDisciplines = data.MainDisciplines != null
                ? _mapper.Map<List<BindMainDisciplineDto>>(data.MainDisciplines)
                : new List<BindMainDisciplineDto>(),
            AdditionalDisciplines = data.AdditionalDisciplines != null
                ? _mapper.Map<List<BindAddDisciplineDto>>(data.AdditionalDisciplines)
                : new List<BindAddDisciplineDto>()
        };
    }
}