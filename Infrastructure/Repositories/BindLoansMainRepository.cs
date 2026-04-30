using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IBindLoansMainRepository
{
    Task<(int TotalCount, List<BindLoansMainDto> Items)> GetPagedAsync(BindLoansMainQueryDto queryDto);
    Task<BindLoansMainDto?> GetDtoByIdAsync(int id);
    Task<BindLoansMain?> GetEntityByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(BindLoansMain entity);
    Task<int> DeleteAsync(int id);
    Task SaveChangesAsync();
}

public class BindLoansMainRepository : IBindLoansMainRepository
{
    private readonly AppDbContext _context;

    public BindLoansMainRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalCount, List<BindLoansMainDto> Items)> GetPagedAsync(BindLoansMainQueryDto queryDto)
    {
        var query = _context.BindLoansMains.AsNoTracking().AsQueryable();

        // 1. ФІЛЬТРАЦІЯ
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(b =>
                (b.AddDisciplines != null && (
                    EF.Functions.Like(b.AddDisciplines.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(b.AddDisciplines.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%"))) ||
                (b.EducationalProgram != null && (
                    EF.Functions.Like(b.EducationalProgram.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(b.EducationalProgram.SpecialityCode.ToLower(), $"%{lowerSearch}%"))));
        }

        if (queryDto.AddDisciplinesIds != null && queryDto.AddDisciplinesIds.Any())
            query = query.Where(b => b.AddDisciplinesId.HasValue && queryDto.AddDisciplinesIds.Contains(b.AddDisciplinesId.Value));

        if (queryDto.EducationalProgramIds != null && queryDto.EducationalProgramIds.Any())
            query = query.Where(b => b.EducationalProgramId.HasValue && queryDto.EducationalProgramIds.Contains(b.EducationalProgramId.Value));

        // 2. ПІДРАХУНОК (до пагінації)
        var totalCount = await query.CountAsync();

        // 3. СОРТУВАННЯ
        query = queryDto.SortOrder switch
        {
            1 => query.OrderByDescending(b => b.AddDisciplines.CodeSelectiveDisciplines),
            2 => query.OrderBy(b => b.EducationalProgram.SpecialityCode),
            3 => query.OrderByDescending(b => b.EducationalProgram.SpecialityCode),
            _ => query.OrderBy(b => b.AddDisciplines.CodeSelectiveDisciplines)
        };

        // 4. БЛИСКАВИЧНА ПРОЕКЦІЯ ТА ПАГІНАЦІЯ
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(b => new BindLoansMainDto
            {
                IdBindLoan = b.IdBindLoan,
                AddDisciplinesId = b.AddDisciplinesId ?? 0,
                EducationalProgramId = b.EducationalProgramId ?? 0,
                CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeSelectiveDisciplines : "",
                AddDisciplineName = b.AddDisciplines != null ? b.AddDisciplines.NameSelectiveDisciplines : "",
                SpecialityCode = b.EducationalProgram != null ? b.EducationalProgram.SpecialityCode : "",
                EducationalProgramName = b.EducationalProgram != null ? b.EducationalProgram.NameEducationalProgram : ""
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<BindLoansMainDto?> GetDtoByIdAsync(int id)
    {
        return await _context.BindLoansMains
            .AsNoTracking()
            .Where(b => b.IdBindLoan == id)
            .Select(b => new BindLoansMainDto
            {
                IdBindLoan = b.IdBindLoan,
                AddDisciplinesId = b.AddDisciplinesId ?? 0,
                EducationalProgramId = b.EducationalProgramId ?? 0,
                CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeSelectiveDisciplines : "",
                AddDisciplineName = b.AddDisciplines != null ? b.AddDisciplines.NameSelectiveDisciplines : "",
                SpecialityCode = b.EducationalProgram != null ? b.EducationalProgram.SpecialityCode : "",
                EducationalProgramName = b.EducationalProgram != null ? b.EducationalProgram.NameEducationalProgram : ""
            })
            .FirstOrDefaultAsync();
    }

    public async Task<BindLoansMain?> GetEntityByIdAsync(int id)
    {
        // Цей метод потрібен для Update, тому тут НЕМАЄ AsNoTracking
        return await _context.BindLoansMains.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.BindLoansMains.AnyAsync(b => b.IdBindLoan == id);
    }

    public async Task AddAsync(BindLoansMain entity)
    {
        await _context.BindLoansMains.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        // Сучасне швидке видалення одним SQL-запитом
        return await _context.BindLoansMains
            .Where(b => b.IdBindLoan == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}