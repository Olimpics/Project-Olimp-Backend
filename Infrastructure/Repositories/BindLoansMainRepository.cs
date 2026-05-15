using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IBindLoansMainRepository
{
    Task<(int TotalCount, List<BindLoansMainDto> Items)> GetPagedAsync(BindLoansMainQueryDto queryDto);
    Task<BindLoansMainDto?> GetDtoByIdAsync(Guid id);
    Task<BindLoansMain?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(BindLoansMain entity);
    Task<int> DeleteAsync(Guid id);
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
                (b.SelectiveDisciplines != null && (
                    EF.Functions.Like(b.SelectiveDisciplines.NameSelectiveDisciplines.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(b.SelectiveDisciplines.CodeSelectiveDisciplines.ToLower(), $"%{lowerSearch}%"))) ||
                (b.EducationalProgram != null && (
                    EF.Functions.Like(b.EducationalProgram.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(b.EducationalProgram.Speciality != null && b.EducationalProgram.Speciality.Code != null ? b.EducationalProgram.Speciality.Code.ToLower() : "", $"%{lowerSearch}%"))));
        }

        if (queryDto.SelectiveDisciplinesIds != null && queryDto.SelectiveDisciplinesIds.Any())
            query = query.Where(b => b.SelectiveDisciplinesId.HasValue && queryDto.SelectiveDisciplinesIds.Contains(b.SelectiveDisciplinesId.Value));

        if (queryDto.EducationalProgramIds != null && queryDto.EducationalProgramIds.Any())
            query = query.Where(b => b.EducationalProgramId.HasValue && queryDto.EducationalProgramIds.Contains(b.EducationalProgramId.Value));

        // 2. ПІДРАХУНОК (до пагінації)
        var totalCount = await query.CountAsync();

        // 3. СОРТУВАННЯ
        query = queryDto.SortOrder switch
        {
            1 => query.OrderByDescending(b => b.SelectiveDisciplines.CodeSelectiveDisciplines),
            2 => query.OrderBy(b => b.EducationalProgram.Speciality.Code),
            3 => query.OrderByDescending(b => b.EducationalProgram.Speciality.Code),
            _ => query.OrderBy(b => b.SelectiveDisciplines.CodeSelectiveDisciplines)
        };

        // 4. БЛИСКАВИЧНА ПРОЕКЦІЯ ТА ПАГІНАЦІЯ
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(b => new BindLoansMainDto
            {
                IdBindLoan = b.IdBindLoanMain,
                SelectiveDisciplinesId = b.SelectiveDisciplinesId ?? Guid.Empty,
                EducationalProgramId = b.EducationalProgramId ?? Guid.Empty,
                CodeSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.CodeSelectiveDisciplines : "",
                SelectiveDisciplineName = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.NameSelectiveDisciplines : "",
                SpecialityCode = b.EducationalProgram != null && b.EducationalProgram.Speciality != null && b.EducationalProgram.Speciality.Code != null ? b.EducationalProgram.Speciality.Code.ToLower() : "",
                EducationalProgramName = b.EducationalProgram != null ? b.EducationalProgram.NameEducationalProgram : ""
            })
            .ToListAsync();

        return (totalCount, items);
    }

    public async Task<BindLoansMainDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.BindLoansMains
            .AsNoTracking()
            .Where(b => b.IdBindLoanMain == id)
            .Select(b => new BindLoansMainDto
            {
                IdBindLoan = b.IdBindLoanMain,
                SelectiveDisciplinesId = b.SelectiveDisciplinesId ?? Guid.Empty,
                EducationalProgramId = b.EducationalProgramId ?? Guid.Empty,
                CodeSelectiveDisciplines = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.CodeSelectiveDisciplines : "",
                SelectiveDisciplineName = b.SelectiveDisciplines != null ? b.SelectiveDisciplines.NameSelectiveDisciplines : "",
                SpecialityCode = b.EducationalProgram != null && b.EducationalProgram.Speciality != null && b.EducationalProgram.Speciality.Code != null ? b.EducationalProgram.Speciality.Code.ToString() : "",
                EducationalProgramName = b.EducationalProgram != null ? b.EducationalProgram.NameEducationalProgram : ""
            })
            .FirstOrDefaultAsync();
    }

    public async Task<BindLoansMain?> GetEntityByIdAsync(Guid id)
    {
        return await _context.BindLoansMains.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.BindLoansMains.AnyAsync(b => b.IdBindLoanMain == id);
    }

    public async Task AddAsync(BindLoansMain entity)
    {
        await _context.BindLoansMains.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        return await _context.BindLoansMains
            .Where(b => b.IdBindLoanMain == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
