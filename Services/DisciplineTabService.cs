using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Linq.Expressions;

namespace OlimpBack.Services;

public class DisciplineTabService : IDisciplineTabService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DisciplineTabService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<object?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null)
            return null;

        var query = _context.AddDisciplines
            .Include(d => d.DegreeLevel)
            .Include(d => d.Faculty)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Faculties))
        {
            var facultyIds = queryDto.Faculties
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.Parse(id.Trim()))
                .ToList();
            query = query.Where(d => facultyIds.Contains(d.FacultyId));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Courses))
        {
            var courseList = queryDto.Courses.Split(',').Select(int.Parse).ToList();
            query = query.Where(d =>
                (!d.MinCourse.HasValue || courseList.Contains(d.MinCourse.Value)) &&
                (!d.MaxCourse.HasValue || courseList.Contains(d.MaxCourse.Value)));
        }

        if (queryDto.IsEvenSemester.HasValue)
        {
            var semesterValue = queryDto.IsEvenSemester.Value ? (sbyte)0 : (sbyte)1;
            query = query.Where(d => d.AddSemestr == semesterValue);
        }

        if (!string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds))
        {
            var levelIds = queryDto.DegreeLevelIds.Split(',').Select(int.Parse).ToList();
            var parameter = Expression.Parameter(typeof(AddDiscipline), "d");
            var property = Expression.Property(parameter, nameof(AddDiscipline.DegreeLevelId));
            var equalsExpressions = levelIds.Select(levelId =>
                Expression.Equal(
                    property,
                    Expression.Constant((int?)levelId, typeof(int?))
                ));
            Expression combinedOr = equalsExpressions.Aggregate((a, b) => Expression.OrElse(a, b));
            var lambda = Expression.Lambda<Func<AddDiscipline, bool>>(combinedOr, parameter);
            query = query.Where(lambda);
        }

        var allDisciplines = await query.ToListAsync();

        var fullList = allDisciplines.Select(discipline =>
        {
            var dto = _mapper.Map<FullDisciplineDto>(discipline);
            dto.CountOfPeople = context.DisciplineCounts.TryGetValue(discipline.IdAddDisciplines, out var c) ? c : 0;
            dto.IsAvailable = DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context);
            return dto;
        }).ToList();

        if (queryDto.OnlyAvailable)
            fullList = fullList.Where(d => d.IsAvailable).ToList();

        fullList = queryDto.SortOrder switch
        {
            1 => fullList.OrderByDescending(d => d.NameAddDisciplines).ToList(),
            2 => fullList.OrderBy(d => d.CountOfPeople).ToList(),
            3 => fullList.OrderByDescending(d => d.CountOfPeople).ToList(),
            4 => fullList.OrderBy(d => d.FacultyAbbreviation).ToList(),
            _ => fullList.OrderBy(d => d.NameAddDisciplines).ToList()
        };

        var totalItems = fullList.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);
        var paginatedResult = fullList
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        return new
        {
            totalPages,
            totalItems,
            currentPage = queryDto.Page,
            pageSize = queryDto.PageSize,
            disciplines = paginatedResult,
            filters = new
            {
                faculties = string.IsNullOrWhiteSpace(queryDto.Faculties) ? null : queryDto.Faculties.Split(',').Select(f => f.Trim()).ToList(),
                courses = string.IsNullOrWhiteSpace(queryDto.Courses) ? null : queryDto.Courses.Split(',').Select(int.Parse).ToList(),
                isEvenSemester = queryDto.IsEvenSemester,
                degreeLevelIds = string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds) ? null : queryDto.DegreeLevelIds.Split(',').Select(int.Parse).ToList()
            }
        };
    }

    public async Task<(object? result, string? error)> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null)
            return (null, "StudentNotFound");

        var now = DateTime.UtcNow;
        var deadline = await _context.DisciplineChoicePeriods
            .Where(p => p.FacultyId == context.Student.FacultyId
                && p.StartDate <= now
                && p.EndDate >= now)
            .FirstOrDefaultAsync();

        if (deadline == null)
            return (null, "PeriodNotConfirmed");

        var disciplines = await _context.AddDisciplines
            .Where(d => d.AddSemestr == (queryDto.IsEvenSemester ? (sbyte)0 : (sbyte)1))
            .ToListAsync();

        var availableDisciplines = disciplines
            .Where(d => DisciplineAvailabilityService.IsDisciplineAvailable(d, context))
            .Select(d => new SimpleDisciplineDto
            {
                IdAddDisciplines = d.IdAddDisciplines,
                NameAddDisciplines = d.NameAddDisciplines,
                CodeAddDisciplines = d.CodeAddDisciplines
            })
            .ToList();

        return (new DisciplineTabResponseDto
        {
            StudentId = context.Student.IdStudent,
            StudentName = context.Student.NameStudent,
            CurrentCourse = context.CurrentCourse,
            IsEvenSemester = queryDto.IsEvenSemester,
            Disciplines = availableDisciplines
        }, null);
    }

    public async Task<(int? bindId, string? error)> AddDisciplineBindAsync(AddDisciplineBindDto dto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(dto.StudentId, _context);
        if (context == null)
            return (null, $"Student not found {dto.StudentId}");

        if (dto.Semester != 0 && dto.Semester != 1)
            return (null, "Semester must be 0 or 1");

        int targetCourse = context.CurrentCourse + 1;
        int targetSemester = (targetCourse * 2) - dto.Semester;

        if (targetCourse > 4)
            return (null, "You can't choose disciplines in 5th course");

        if (targetSemester > 8)
            return (null, $"Invalid semester: {targetSemester}");

        var discipline = await _context.AddDisciplines
            .FirstOrDefaultAsync(d => d.IdAddDisciplines == dto.DisciplineId);

        if (discipline == null)
            return (null, "Discipline not found");

        if (context.BoundDisciplineIds.Contains(dto.DisciplineId))
            return (null, "Student is already enrolled in this discipline");

        if (!DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context))
            return (null, "Discipline is not available for this student");

        var bind = new BindAddDiscipline
        {
            StudentId = dto.StudentId,
            AddDisciplinesId = dto.DisciplineId,
            Semestr = targetSemester,
            InProcess = 1,
            Loans = 5
        };

        _context.BindAddDisciplines.Add(bind);
        await _context.SaveChangesAsync();

        return (bind.IdBindAddDisciplines, null);
    }

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(int id)
    {
        var discipline = await _context.AddDisciplines
            .Include(d => d.DegreeLevel)
            .Include(d => d.AddDetail)
                .ThenInclude(d => d.Department)
            .Include(d => d.Faculty)
            .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

        if (discipline == null || discipline.AddDetail == null)
            return null;

        return _mapper.Map<FullDisciplineWithDetailsDto>((discipline, discipline.AddDetail));
    }

    public async Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateAddDisciplineWithDetailsDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var discipline = _mapper.Map<AddDiscipline>(dto, opts => opts.Items["DbContext"] = _context);
            _context.AddDisciplines.Add(discipline);
            await _context.SaveChangesAsync();

            var details = _mapper.Map<AddDetail>(dto.Details);
            details.IdAddDetails = discipline.IdAddDisciplines;
            _context.AddDetails.Add(details);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var createdDiscipline = await _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .Include(d => d.AddDetail)
                    .ThenInclude(d => d.Department)
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == discipline.IdAddDisciplines);

            if (createdDiscipline == null || createdDiscipline.AddDetail == null)
                return null;

            return _mapper.Map<FullDisciplineWithDetailsDto>((createdDiscipline, createdDiscipline.AddDetail));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(int id, UpdateAddDisciplineWithDetailsDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var discipline = await _context.AddDisciplines
                .Include(d => d.AddDetail)
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

            if (discipline == null)
                return (false, "Discipline not found");

            if (dto.Details.DepartmentId.HasValue)
            {
                var departmentExists = await _context.Departments
                    .AnyAsync(d => d.IdDepartment == dto.Details.DepartmentId.Value);
                if (!departmentExists)
                    return (false, $"Department with ID {dto.Details.DepartmentId.Value} does not exist");
            }

            if (discipline.AddDetail == null)
            {
                discipline.AddDetail = new AddDetail
                {
                    IdAddDetails = discipline.IdAddDisciplines
                };
                _context.AddDetails.Add(discipline.AddDetail);
            }

            _mapper.Map(dto, discipline, opts => opts.Items["DbContext"] = _context);
            _mapper.Map(dto.Details, discipline.AddDetail);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, null);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteDisciplineWithDetailsAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var discipline = await _context.AddDisciplines
                .Include(d => d.AddDetail)
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

            if (discipline == null)
                return false;

            if (discipline.AddDetail != null)
                _context.AddDetails.Remove(discipline.AddDetail);

            _context.AddDisciplines.Remove(discipline);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
