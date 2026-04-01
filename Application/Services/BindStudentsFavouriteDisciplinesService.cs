using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IBindStudentsFavouriteDisciplinesService
{
    Task<IEnumerable<StudentFavouriteDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId);
    Task<(bool success, int statusCode, string? errorMessage, StudentFavouriteDisciplineDto? dto)> AddFavoriteAsync(AddFavoriteDisciplineDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> RemoveFavoriteAsync(int bindId);
}

public class BindStudentsFavouriteDisciplinesService : IBindStudentsFavouriteDisciplinesService
{
    private readonly IBindStudentsFavouriteDisciplinesRepository _repository;

    public BindStudentsFavouriteDisciplinesService(IBindStudentsFavouriteDisciplinesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<StudentFavouriteDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId)
    {
        // Просто викликаємо наш оптимізований метод з репозиторію
        return await _repository.GetFavoriteDiciplinesByStudentAsync(studentId);
    }

    public async Task<(bool success, int statusCode, string? errorMessage, StudentFavouriteDisciplineDto? dto)> AddFavoriteAsync(AddFavoriteDisciplineDto dto)
    {
        // 1. Перевіряємо, чи немає вже такої дисципліни в обраному цього студента
        if (await _repository.ExistsAsync(dto.StudentId, dto.DisciplineId))
        {
            return (false, StatusCodes.Status400BadRequest, "This discipline is already in favorites for this student.", null);
        }

        // 2. Створюємо сутність
        var entity = new BindStudentsFavouriteDiscipline
        {
            IdStudent = dto.StudentId,
            IdAddDiscipline = dto.DisciplineId
        };

        // 3. Зберігаємо
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // 4. Витягуємо свіжостворений запис через ProjectTo, щоб повернути його на фронтенд
        var resultDto = await _repository.GetFavoriteByIdAsync(entity.IdBindStudentsFavouriteDisciplines);

        return (true, StatusCodes.Status201Created, null, resultDto);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> RemoveFavoriteAsync(int bindId)
    {
        var deletedRows = await _repository.DeleteAsync(bindId);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Favorite record not found.");

        return (true, StatusCodes.Status204NoContent, null);
    }
}