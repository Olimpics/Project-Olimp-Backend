using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Linq.Expressions;

namespace OlimpBack.Application.Services;

public class StudentService : IStudentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<StudentForCatalogDto>> GetStudentsAsync(StudentQueryDto queryDto)
    {
        // Відключаємо трекінг, Include нам більше не потрібні завдяки ProjectTo!
        var query = _context.Students.AsNoTracking().AsQueryable();

        // 1. ПОШУК І ФІЛЬТРАЦІЯ НА РІВНІ БД
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s => EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
        {
            var numericValues = queryDto.Faculties.Where(f => int.TryParse(f, out _)).Select(int.Parse).ToList();
            var textValues = queryDto.Faculties.Where(f => !int.TryParse(f, out _)).Select(f => f.ToLower()).ToList();

            // Динамічний OR для факультетів
            query = query.Where(s =>
                numericValues.Contains(s.FacultyId) ||
                s.Faculty != null && textValues.Any(t => s.Faculty.NameFaculty.ToLower().Contains(t) || s.Faculty.Abbreviation.ToLower().Contains(t))
            );
        }

        if (queryDto.Specialities != null && queryDto.Specialities.Any())
        {
            // Спрощене Expression Tree для спеціальностей (StartsWith -> Like 'val%')
            var parameter = Expression.Parameter(typeof(Student), "s");
            var progProp = Expression.Property(parameter, nameof(Student.EducationalProgram));
            var specCodeProp = Expression.Property(progProp, nameof(EducationalProgram.SpecialityCode));

            Expression? orExpression = null;
            foreach (var val in queryDto.Specialities.Select(s => s.ToLower()))
            {
                var likeCall = Expression.Call(
                    typeof(DbFunctionsExtensions), nameof(DbFunctionsExtensions.Like), Type.EmptyTypes,
                    Expression.Constant(EF.Functions), specCodeProp, Expression.Constant(val + "%")
                );
                orExpression = orExpression == null ? likeCall : Expression.OrElse(orExpression, likeCall);
            }
            query = query.Where(Expression.Lambda<Func<Student, bool>>(orExpression!, parameter));
        }

        if (queryDto.GroupIds != null && queryDto.GroupIds.Any())
            query = query.Where(s => queryDto.GroupIds.Contains(s.GroupId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(s => queryDto.Courses.Contains(s.Course));

        if (queryDto.StudyFormIds != null && queryDto.StudyFormIds.Any())
            query = query.Where(s => queryDto.StudyFormIds.Contains(s.StudyFormId));

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(s => queryDto.DegreeLevelIds.Contains(s.EducationalDegreeId));

        if (queryDto.IsShort.HasValue)
        {
            query = query.Where(s => s.IsShort == queryDto.IsShort.Value);
        }

        // 2. СОРТУВАННЯ НА РІВНІ БД (До ToListAsync!)
        query = queryDto.SortOrder switch
        {
            1 => query.OrderByDescending(d => d.NameStudent),
            2 => query.OrderBy(d => d.Faculty.Abbreviation),
            3 => query.OrderByDescending(d => d.Faculty.Abbreviation),
            4 => query.OrderBy(d => d.Group.GroupCode),
            5 => query.OrderByDescending(d => d.Group.GroupCode),
            _ => query.OrderBy(d => d.NameStudent)
        };

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        // 3. БЛИСКАВИЧНА ПАГІНАЦІЯ І ПРОЕКЦІЯ
        // Ми одразу конвертуємо SQL у DTO, минаючи важкі моделі
        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ProjectTo<StudentForCatalogDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PaginatedResponseDto<StudentForCatalogDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = queryDto // Віддаємо просто сам об'єкт, він вже чистий
        };
    }

    public async Task<StudentDto?> GetStudentAsync(int id)
    {
        // Ти тут вже написав ідеально! Додав лише AsNoTracking для швидкості
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == id)
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<StudentDto>> CreateStudentsAsync(IReadOnlyList<CreateStudentDto> dtos)
    {
        // 4. ОПТИМІЗАЦІЯ ПАКЕТНОГО ВСТАВЛЕННЯ (BULK INSERT)
        var results = new List<StudentDto>();
        var studentsToAdd = new List<Student>();

        // Перевіряємо всіх існуючих студентів ОДНИМ запитом, а не в циклі
        var namesToCheck = dtos.Where(d => !string.IsNullOrWhiteSpace(d.NameStudent)).Select(d => d.NameStudent).ToList();
        var existingStudents = await _context.Students
            .Where(s => namesToCheck.Contains(s.NameStudent))
            .Select(s => new { s.IdStudent, s.NameStudent })
            .ToListAsync();

        foreach (var dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.NameStudent))
                continue;

            // Перевіряємо в пам'яті (миттєво)
            if (existingStudents.Any(s => s.NameStudent == dto.NameStudent && s.IdStudent == dto.IdStudent))
                continue;

            var userId = dto.UserId;
            if (userId == 0)
            {
                // Залишив твій сервіс, припускаємо що він зберігає юзера
                userId = await UserService.CreateUserForStudent(dto.NameStudent, _context);
            }
            dto.UserId = userId;

            var student = _mapper.Map<Student>(dto);
            studentsToAdd.Add(student);
        }

        if (studentsToAdd.Any())
        {
            // Додаємо всіх разом і зберігаємо ОДИН раз!
            _context.Students.AddRange(studentsToAdd);
            await _context.SaveChangesAsync();

            results.AddRange(_mapper.Map<List<StudentDto>>(studentsToAdd));
        }

        return results;
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateStudentAsync(int id, UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, "Student not found");

        _mapper.Map(dto, student);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Students.AnyAsync(s => s.IdStudent == id);
            if (!exists)
                return (false, StatusCodes.Status404NotFound, "Student not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }
}