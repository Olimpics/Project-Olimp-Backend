using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Application.Services;

public interface IStudentPageService
{
    Task<StudentAddDisciplinesDto?> GetStudentAddDisciplinesAsync(int studentId);
    Task<StudentEducationalProgramDto?> GetStudentEducationalProgramAsync(int studentId);
}

public class StudentPageService : IStudentPageService
{
    private readonly IStudentPageRepository _repository;

    public StudentPageService(IStudentPageRepository repository)
    {
        _repository = repository;
    }

     public async Task<StudentEducationalProgramDto?> GetStudentEducationalProgramAsync(int studentId)
    {
        return await _repository.GetStudentEducationalProgramAsync(studentId);
    }

    public async Task<StudentAddDisciplinesDto?> GetStudentAddDisciplinesAsync(int studentId)
    {
        return await _repository.GetStudentAddDisciplinesAsync(studentId);
    }
}