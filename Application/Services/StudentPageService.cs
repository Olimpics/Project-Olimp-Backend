using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Application.Services;

public interface IStudentPageService
{
    Task<StudentDisciplinesDto?> GetStudentDisciplinesAsync(int studentId);
}

public class StudentPageService : IStudentPageService
{
    private readonly IStudentPageRepository _repository;

    public StudentPageService(IStudentPageRepository repository)
    {
        _repository = repository;
    }

    public async Task<StudentDisciplinesDto?> GetStudentDisciplinesAsync(int studentId)
    {
        return await _repository.GetStudentDisciplinesAsync(studentId);
    }
}