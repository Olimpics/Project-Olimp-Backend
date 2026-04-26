using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories; // Твій namespace репозиторію
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class BindLoansMainService : IBindLoansMainService
{
    private readonly IBindLoansMainRepository _repository;

    public BindLoansMainService(IBindLoansMainRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResponseDto<BindLoansMainDto>> GetBindLoansMainAsync(BindLoansMainQueryDto queryDto)
    {
        // Сервіс просто делегує важку роботу репозиторію
        var (totalCount, items) = await _repository.GetPagedAsync(queryDto);

        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        // Формуємо фінальну відповідь
        return new PaginatedResponseDto<BindLoansMainDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = queryDto // Віддаємо DTO назад як фільтри
        };
    }

    public async Task<BindLoansMainDto?> GetBindLoansMainAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<BindLoansMainDto> CreateBindLoansMainAsync(CreateBindLoansMainDto dto)
    {
        var binding = new BindLoansMain
        {
            AddDisciplinesId = dto.AddDisciplinesId,
            EducationalProgramId = dto.EducationalProgramId
        };

        await _repository.AddAsync(binding);
        await _repository.SaveChangesAsync();

        // Більше не робимо повторний запит (re-fetch) у БД!
        // ID згенерувався після SaveChangesAsync, а інші дані ми і так знаємо.
        // Це зекономить 1 похід у базу при кожному створенні.
        return new BindLoansMainDto
        {
            IdBindLoan = binding.IdBindLoan.GetValueOrDefault(),
            AddDisciplinesId = binding.AddDisciplinesId.GetValueOrDefault(),
            EducationalProgramId = binding.EducationalProgramId.GetValueOrDefault(),
            CodeAddDisciplines = "", // При створенні ми не знаємо назв, і це нормально для response
            AddDisciplineName = "",
            SpecialityCode = "",
            EducationalProgramName = ""
        };
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateBindLoansMainAsync(int id, UpdateBindLoansMainDto dto)
    {
        // Бізнес-перевірка залишається в сервісі
        if (id != dto.IdBindLoan)
            return (false, StatusCodes.Status400BadRequest, "Route id does not match body id.");

        var binding = await _repository.GetEntityByIdAsync(id);
        if (binding == null)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        // Оновлюємо поля
        binding.AddDisciplinesId = dto.AddDisciplinesId;
        binding.EducationalProgramId = dto.EducationalProgramId;

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                return (false, StatusCodes.Status404NotFound, "Binding not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteBindLoansMainAsync(int id)
    {
        // Сучасне видалення через репозиторій
        var deletedRows = await _repository.DeleteAsync(id);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        return (true, StatusCodes.Status204NoContent, null);
    }
}