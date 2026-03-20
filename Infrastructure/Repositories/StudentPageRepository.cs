using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IStudentPageRepository
{
    Task<StudentEducationalProgramDto?> GetStudentEducationalProgramAsync(int studentId);
    Task<StudentAddDisciplinesDto?> GetStudentAddDisciplinesAsync(int studentId);
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
                MainDisciplines = s.EducationalProgram != null ? s.EducationalProgram.BindMainDisciplines : null,
                AdditionalDisciplines = s.BindAddDisciplines
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return null;

        return new StudentEducationalProgramDto
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
    public async Task<StudentAddDisciplinesDto?> GetStudentAddDisciplinesAsync(int studentId)
    {
        var data = await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId)
            .Select(s => new
            {
                s.IdStudent,
                s.NameStudent,
                AdditionalDisciplines = s.BindAddDisciplines
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return null;

        return new StudentAddDisciplinesDto
        {
            StudentId = data.IdStudent,
            StudentName = data.NameStudent ?? "",
            AdditionalDisciplines = data.AdditionalDisciplines != null
                ? _mapper.Map<List<BindAddDisciplineDto>>(data.AdditionalDisciplines)
                : new List<BindAddDisciplineDto>()
        };
    }
}