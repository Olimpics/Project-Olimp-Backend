using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IStudentPageRepository
{
    Task<StudentEducationalProgramDto?> GetStudentEducationalProgramAsync(int studentId);
    Task<StudentSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(int studentId);
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

    public async Task<StudentEducationalProgramDto?> GetStudentEducationalProgramAsync(int studentId)
    {
        var data = await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId)
            .Select(s => new
            {
                s.IdStudent,
                s.NameStudent,
                MainDisciplines = s.EducationalProgram != null ? s.EducationalProgram.MainDisciplines : null,
                AdditionalDisciplines = s.BindSelectiveDisciplines
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return null;

        return new StudentEducationalProgramDto
        {
            StudentId = data.IdStudent,
            StudentName = data.NameStudent ?? "",
            MainDisciplines = data.MainDisciplines != null
                ? _mapper.Map<List<MainDisciplineDto>>(data.MainDisciplines)
                : new List<MainDisciplineDto>(),
            AdditionalDisciplines = data.AdditionalDisciplines != null
                ? _mapper.Map<List<BindSelectiveDisciplineDto>>(data.AdditionalDisciplines)
                : new List<BindSelectiveDisciplineDto>()
        };
    }
    public async Task<StudentSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(int studentId)
    {
        var data = await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId)
            .Select(s => new
            {
                s.IdStudent,
                s.NameStudent,
                AdditionalDisciplines = s.BindSelectiveDisciplines
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return null;

        return new StudentSelectiveDisciplinesDto
        {
            StudentId = data.IdStudent,
            StudentName = data.NameStudent ?? "",
            AdditionalDisciplines = data.AdditionalDisciplines != null
                ? _mapper.Map<List<BindSelectiveDisciplineDto>>(data.AdditionalDisciplines)
                : new List<BindSelectiveDisciplineDto>()
        };
    }
}