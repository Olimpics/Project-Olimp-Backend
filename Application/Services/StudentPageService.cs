using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Application.Services;

public interface IStudentPageService
{
    Task<StudentSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(Guid studentId);
    Task<StudentEducationalProgramDto?> GetStudentEducationalProgramAsync(Guid studentId);
}

public class StudentPageService : IStudentPageService
{
    private readonly IStudentPageRepository _repository;

    public StudentPageService(IStudentPageRepository repository)
    {
        _repository = repository;
    }

     public async Task<StudentEducationalProgramDto?> GetStudentEducationalProgramAsync(Guid studentId)
    {
        return await _repository.GetStudentEducationalProgramAsync(studentId);
    }

    public async Task<StudentSelectiveDisciplinesDto?> GetStudentSelectiveDisciplinesAsync(Guid studentId)
    {
        return await _repository.GetStudentSelectiveDisciplinesAsync(studentId);
    }
}