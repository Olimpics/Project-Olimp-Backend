using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Application.Services;

public interface IBindStudentsFavouriteDisciplinesService
{
    Task<IEnumerable<AddDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId);
}

public class BindStudentsFavouriteDisciplinesService : IBindStudentsFavouriteDisciplinesService
{
    private readonly IBindStudentsFavouriteDisciplinesRepository _repository;

    public BindStudentsFavouriteDisciplinesService(IBindStudentsFavouriteDisciplinesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AddDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId)
    {
        // Просто викликаємо наш оптимізований метод з репозиторію
        return await _repository.GetFavoriteDiciplinesByStudentAsync(studentId);
    }
}