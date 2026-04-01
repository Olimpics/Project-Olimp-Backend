using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;


public interface IBindStudentsFavouriteDisciplinesRepository
{
    Task<IEnumerable<AddDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId);
}

public class BindStudentsFavouriteDisciplinesRepository : IBindStudentsFavouriteDisciplinesRepository
{
    private readonly AppDbContext _context;
    public BindStudentsFavouriteDisciplinesRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<AddDisciplineDto>> GetFavoriteDiciplinesByStudentAsync(int studentId)
    {

        var student = await _context.BindStudentsFavouriteDisciplines
            .AsNoTracking()
            .Where(s => s.IdStudent == studentId)
            .Select()
            .FirstOrDefaultAsync();
    }
}