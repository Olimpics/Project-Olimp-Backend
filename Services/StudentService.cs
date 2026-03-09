using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Linq.Expressions;

namespace OlimpBack.Services;

public class StudentService : IStudentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<StudentForCatalogDto>> GetStudentsAsync(
        StudentQueryDto queryDto)
    {
        var query = _context.Students
            .Include(s => s.EducationStatus)
            .Include(s => s.Faculty)
            .Include(s => s.EducationalProgram)
            .Include(s => s.EducationalDegree)
            .Include(s => s.Group)
            .Include(s => s.StudyForm)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s =>
                EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%"));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Faculties))
        {
            var facultyValues = queryDto.Faculties
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var numericValues = facultyValues
                .Where(f => int.TryParse(f, out _))
                .Select(int.Parse)
                .ToList();

            var textValues = facultyValues
                .Where(f => !int.TryParse(f, out _))
                .Select(f => f.ToLower())
                .ToList();

            if (numericValues.Any())
            {
                query = query.Where(s => numericValues.Contains(s.FacultyId));
            }

            if (textValues.Any())
            {
                query = query.Where(s =>
                    textValues.Any(t =>
                        EF.Functions.Like(s.Faculty.NameFaculty.ToLower(), $"%{t}%") ||
                        EF.Functions.Like(s.Faculty.Abbreviation.ToLower(), $"%{t}%")));
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Speciality))
        {
            var specialityValues = queryDto.Speciality
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => f.ToLower())
                .ToList();

            if (specialityValues.Any())
            {
                var parameter = Expression.Parameter(typeof(Student), "s");
                var property = Expression.Property(
                    Expression.Property(parameter, nameof(Student.EducationalProgram)),
                    nameof(EducationalProgram.SpecialityCode)
                );

                var toLowerCall = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

                Expression? combinedExpression = null;
                foreach (var val in specialityValues)
                {
                    var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
                    var startsWithCall = Expression.Call(toLowerCall, startsWithMethod, Expression.Constant(val));

                    combinedExpression = combinedExpression == null
                        ? startsWithCall
                        : Expression.OrElse(combinedExpression, startsWithCall);
                }

                var lambda = Expression.Lambda<Func<Student, bool>>(combinedExpression!, parameter);
                query = query.Where(lambda);
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Group))
        {
            var groupIdList = queryDto.Group
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(g => int.TryParse(g, out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (groupIdList.Any())
            {
                query = query.Where(s => groupIdList.Contains(s.Group.IdGroup));
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Courses))
        {
            var courseList = queryDto.Courses
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse)
                .ToList();
            query = query.Where(s => courseList.Contains(s.Course));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.StudyForm))
        {
            var studyFormIds = queryDto.StudyForm
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse)
                .ToList();
            query = query.Where(s => studyFormIds.Contains(s.StudyFormId));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds))
        {
            var levelIds = queryDto.DegreeLevelIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse)
                .ToList();
            query = query.Where(s => levelIds.Contains(s.EducationalDegreeId));
        }

        var students = await query.ToListAsync();

        students = queryDto.SortOrder switch
        {
            1 => students.OrderByDescending(d => d.NameStudent).ToList(),
            2 => students.OrderBy(d => d.Faculty.Abbreviation).ToList(),
            3 => students.OrderByDescending(d => d.Faculty.Abbreviation).ToList(),
            4 => students.OrderBy(d => d.Group.GroupCode).ToList(),
            5 => students.OrderByDescending(d => d.Group.GroupCode).ToList(),
            _ => students.OrderBy(d => d.NameStudent).ToList()
        };

        var totalItems = students.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        var paginatedResult = students
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        var items = _mapper.Map<List<StudentForCatalogDto>>(paginatedResult);

        return new PaginatedResponseDto<StudentForCatalogDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = new
            {
                faculties = queryDto.Faculties?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                queryDto.Speciality,
                queryDto.Group,
                courses = queryDto.Courses?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList(),
                studyForm = queryDto.StudyForm?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList(),
                degreeLevelIds = queryDto.DegreeLevelIds?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList()
            }
        };
    }

    public async Task<StudentDto?> GetStudentAsync(int id)
    {
        var student = await _context.Students
            .Where(s => s.IdStudent == id)
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return student;
    }

    public async Task<IReadOnlyList<StudentDto>> CreateStudentsAsync(IReadOnlyList<CreateStudentDto> dtos)
    {
        var results = new List<StudentDto>();

        foreach (var dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.NameStudent))
                continue;

            var existing = await _context.Students
                .FirstOrDefaultAsync(s => s.NameStudent == dto.NameStudent && s.IdStudent == dto.IdStudent);

            if (existing != null)
                continue;

            var userId = dto.UserId;
            if (userId == 0)
                userId = await UserService.CreateUserForStudent(dto.NameStudent, _context);

            dto.UserId = userId;

            var student = _mapper.Map<Student>(dto);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var createdDto = _mapper.Map<StudentDto>(student);
            results.Add(createdDto);
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

