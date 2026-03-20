using AutoMapper;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;
using OlimpBack.Utils; // Для DisciplineAvailabilityService

namespace OlimpBack.Application.Services;

public class DisciplineTabService : IDisciplineTabService
{
    private readonly IDisciplineTabRepository _repository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context; // Тимчасово для BuildAvailabilityContext, якщо його ще не відрефакторили

    public DisciplineTabService(IDisciplineTabRepository repository, IMapper mapper, AppDbContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null) return null;

        var allDisciplines = await _repository.GetDisciplinesForAvailabilityAsync(queryDto);

        var fullList = allDisciplines.Select(discipline =>
        {
            var dto = _mapper.Map<FullDisciplineDto>(discipline);
            dto.CountOfPeople = context.DisciplineCounts.TryGetValue(discipline.IdAddDisciplines, out var c) ? c : 0;
            dto.IsAvailable = DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context);
            return dto;
        });

        if (queryDto.OnlyAvailable)
            fullList = fullList.Where(d => d.IsAvailable);

        var sortedList = queryDto.SortOrder switch
        {
            1 => fullList.OrderByDescending(d => d.NameAddDisciplines).ToList(),
            2 => fullList.OrderBy(d => d.CountOfPeople).ToList(),
            3 => fullList.OrderByDescending(d => d.CountOfPeople).ToList(),
            4 => fullList.OrderBy(d => d.FacultyAbbreviation).ToList(),
            _ => fullList.OrderBy(d => d.NameAddDisciplines).ToList()
        };

        var totalItems = sortedList.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);
        var paginatedResult = sortedList
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        return new PaginatedResponseDto<FullDisciplineDto>
        {
            TotalPages = totalPages,
            TotalItems = totalItems,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = paginatedResult
        };
    }
    public async Task<DisciplineTabResponseDto?> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null) return null;

        var now = DateTime.UtcNow;

        var isPeriodActive = await _repository.IsChoicePeriodActiveAsync(context.Student.FacultyId, now);
        if (!isPeriodActive) return null;

        var disciplines = await _repository.GetDisciplinesBySemesterAsync(queryDto);

        var availableDisciplines = disciplines
            .Where(d => DisciplineAvailabilityService.IsDisciplineAvailable(d, context))
            .Select(d => new SimpleDisciplineDto
            {
                IdAddDisciplines = d.IdAddDisciplines,
                NameAddDisciplines = d.NameAddDisciplines,
                CodeAddDisciplines = d.CodeAddDisciplines
            })
            .ToList();

        return new DisciplineTabResponseDto
        {
            StudentId = context.Student.IdStudent,
            StudentName = context.Student.NameStudent,
            CurrentCourse = context.CurrentCourse,
            IsEvenSemester = queryDto.IsEvenSemester,
            Disciplines = availableDisciplines
        };
    }

    public async Task<(int? bindId, string? error)> AddDisciplineBindAsync(AddDisciplineBindDto dto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(dto.StudentId, _context);
        if (context == null) return (null, $"Student not found {dto.StudentId}");
        if (dto.Semestr != 0 && dto.Semestr != 1) return (null, "Semestr must be 0 or 1");

        int targetCourse = context.CurrentCourse + 1;
        int targetSemester = targetCourse * 2 - dto.Semestr;

        if (targetCourse > 4) return (null, "You can't choose disciplines in 5th course");
        if (targetSemester > 8) return (null, $"Invalid semester: {targetSemester}");
        if (context.BoundDisciplineIds.Contains(dto.DisciplineId)) return (null, "Student is already enrolled");

        var discipline = await _repository.GetDisciplineByIdAsNoTrackingAsync(dto.DisciplineId);
        if (discipline == null) return (null, "Discipline not found");
        if (!DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context))
            return (null, "Discipline is not available for this student");

        var bind = new BindAddDiscipline
        {
            StudentId = dto.StudentId,
            AddDisciplinesId = dto.DisciplineId,
            Semestr = targetSemester,
            InProcess = 1,
            Loans = dto.Loans
        };

        await _repository.AddBindAsync(bind);
        await _repository.SaveChangesAsync();

        return (bind.IdBindAddDisciplines, null);
    }

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(int id) =>
        await _repository.GetDisciplineWithDetailsDtoAsync(id);

    public async Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateAddDisciplineWithDetailsDto dto)
    {
        var discipline = _mapper.Map<AddDiscipline>(dto, opts => opts.Items["DbContext"] = _context);
        var details = _mapper.Map<AddDetail>(dto.Details);

        discipline.AddDetail = details;

        await _repository.AddDisciplineAsync(discipline);
        await _repository.SaveChangesAsync();

        return _mapper.Map<FullDisciplineWithDetailsDto>((discipline, details));
    }

    public async Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(int id, UpdateAddDisciplineWithDetailsDto dto)
    {
        var discipline = await _repository.GetDisciplineWithDetailEntityAsync(id);
        if (discipline == null) return (false, "Discipline not found");

        if (dto.Details.DepartmentId.HasValue)
        {
            if (!await _repository.DepartmentExistsAsync(dto.Details.DepartmentId.Value))
                return (false, $"Department with ID {dto.Details.DepartmentId.Value} does not exist");
        }

        if (discipline.AddDetail == null)
        {
            discipline.AddDetail = new AddDetail { IdAddDetails = discipline.IdAddDisciplines };
            // EF Core track-атиме створення автоматично через навігаційну властивість
        }

        _mapper.Map(dto, discipline, opts => opts.Items["DbContext"] = _context);
        _mapper.Map(dto.Details, discipline.AddDetail);

        await _repository.SaveChangesAsync();

        return (true, null);
    }
}