using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Application.Services;

public interface IStudentPageService
{
    Task<StudentSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(int studentId);
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

    public async Task<StudentSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(int studentId)
    {
        return await _repository.GetStudentSelectiveDisciplinesAsync(studentId);
    }
}