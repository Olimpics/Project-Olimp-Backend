using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IFavoriteDisciplineService
{
    Task<List<FavoriteDisciplineDto>> GetByStudentAsync(Guid studentId);
    Task<(bool success, string? error)> AddAsync(Guid studentId, Guid disciplineId);
    Task<(bool success, string? error)> RemoveAsync(Guid studentId, Guid disciplineId);
}

public class FavoriteDisciplineService : IFavoriteDisciplineService
{
    private readonly AppDbContext _context;

    public FavoriteDisciplineService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FavoriteDisciplineDto>> GetByStudentAsync(Guid studentId)
    {
        var favorites = await _context.BindStudentFavoriteDiscilines
            .AsNoTracking()
            .Include(f => f.SelectiveDiscipline).ThenInclude(d => d.Catalog)
            .Include(f => f.SelectiveDiscipline).ThenInclude(d => d.Department)
            .Where(f => f.StudentId == studentId)
            .ToListAsync();

        var result = new List<FavoriteDisciplineDto>();
        foreach (var fav in favorites)
        {
            var discipline = fav.SelectiveDiscipline;
            int yearStart = discipline.Catalog?.YearStart ?? 0;

            result.Add(new FavoriteDisciplineDto
            {
                Id = fav.IdBindStudentFavoriteDiscipline,
                DisciplineId = discipline.IdSelectiveDisciplines,
                Name = discipline.NameSelectiveDisciplines,
                Code = discipline.CodeSelectiveDisciplines,
                DepartmentName = discipline.Department?.NameDepartment,
                CatalogYearStart = yearStart,
                CatalogYearEnd = discipline.Catalog?.YearEnd ?? 0,
                Similar = await GetSimilarFromNewerCatalogsAsync(discipline.IdSelectiveDisciplines, yearStart)
            });
        }
        return result;
    }

    /// <summary>
    /// Recommends analogous disciplines from catalogs newer than the favorite's own catalog,
    /// using the existing selective similarity groups.
    /// </summary>
    private async Task<List<SimilarDisciplineDto>> GetSimilarFromNewerCatalogsAsync(Guid disciplineId, int yearStart)
    {
        var groupIds = await _context.BindSimilarSelectiveInGroups
            .Where(b => b.SelectiveId == disciplineId)
            .Select(b => b.GroupId)
            .ToListAsync();

        if (!groupIds.Any()) return new List<SimilarDisciplineDto>();

        return await _context.BindSimilarSelectiveInGroups
            .AsNoTracking()
            .Where(b => groupIds.Contains(b.GroupId)
                        && b.SelectiveId != disciplineId
                        && b.Selective.Catalog != null
                        && b.Selective.Catalog.YearStart > yearStart)
            .Select(b => new SimilarDisciplineDto
            {
                Id = b.Selective.IdSelectiveDisciplines,
                Name = b.Selective.NameSelectiveDisciplines,
                YearStart = b.Selective.Catalog.YearStart,
                YearEnd = b.Selective.Catalog.YearEnd
            })
            .Distinct()
            .ToListAsync();
    }

    public async Task<(bool success, string? error)> AddAsync(Guid studentId, Guid disciplineId)
    {
        if (!await _context.Students.AnyAsync(s => s.IdStudent == studentId))
            return (false, "Student not found");
        if (!await _context.SelectiveDisciplines.AnyAsync(d => d.IdSelectiveDisciplines == disciplineId))
            return (false, "Discipline not found");

        bool exists = await _context.BindStudentFavoriteDiscilines
            .AnyAsync(f => f.StudentId == studentId && f.SelectiveDisciplineId == disciplineId);
        if (exists) return (false, "Discipline is already in favorites");

        _context.BindStudentFavoriteDiscilines.Add(new BindStudentFavoriteDisciline
        {
            StudentId = studentId,
            SelectiveDisciplineId = disciplineId
        });
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> RemoveAsync(Guid studentId, Guid disciplineId)
    {
        var fav = await _context.BindStudentFavoriteDiscilines
            .FirstOrDefaultAsync(f => f.StudentId == studentId && f.SelectiveDisciplineId == disciplineId);
        if (fav == null) return (false, "Favorite not found");

        _context.BindStudentFavoriteDiscilines.Remove(fav);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
