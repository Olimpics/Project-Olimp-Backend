using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;

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

    public async Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null)
            return null;

        // Використовуємо AsNoTracking(), бо ми лише читаємо дані! Це значно швидше.
        var query = _context.AddDisciplines
            .Include(d => d.DegreeLevel)
            .Include(d => d.Faculty)
            .AsNoTracking()
            .AsQueryable();

        // 1. ФІЛЬТРАЦІЯ БЕЗ SPLIT
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => queryDto.Faculties.Contains(d.FacultyId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(d =>
                (!d.MinCourse.HasValue || queryDto.Courses.Contains(d.MinCourse.Value)) &&
                (!d.MaxCourse.HasValue || queryDto.Courses.Contains(d.MaxCourse.Value)));

        if (queryDto.IsEvenSemester.HasValue)
        {
            var semesterValue = queryDto.IsEvenSemester.Value ? (sbyte)0 : (sbyte)1;
            query = query.Where(d => d.AddSemestr == semesterValue);
        }

        // 2. ЗНИЩУЄМО ЖАХ З EXPRESSION TREES
        // Тобі більше не треба вручну будувати Expression для порівняння масивів!
        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));

        // Примітка: Оскільки IsAvailable і DisciplineCounts обчислюються в пам'яті (через зовнішній сервіс), 
        // ми змушені вивантажити відфільтровані дані через ToListAsync().
        var allDisciplines = await query.ToListAsync();

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

        var totalItems = sortedList.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);
        var paginatedResult = sortedList
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        // Повертаємо типізований PaginatedResponseDto (потрібно змінити тип повернення методу)
        return new PaginatedResponseDto<FullDisciplineDto>
        {
            TotalPages = totalPages,
            TotalItems = totalItems,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = paginatedResult
        };
    }

    public async Task<(int? bindId, string? error)> AddDisciplineBindAsync(AddDisciplineBindDto dto)
    {
        // ... (Цей метод був написаний досить добре, я залишу його майже без змін, 
        // хіба що додав би AsNoTracking() для discipline, оскільки ми його не змінюємо)

        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(dto.StudentId, _context);
        if (context == null) return (null, $"Student not found {dto.StudentId}");
        if (dto.Semestr != 0 && dto.Semestr != 1) return (null, "Semestr must be 0 or 1");

        int targetCourse = context.CurrentCourse + 1;
        int targetSemester = (targetCourse * 2) - dto.Semestr;

        if (targetCourse > 4) return (null, "You can't choose disciplines in 5th course");
        if (targetSemester > 8) return (null, $"Invalid semester: {targetSemester}");
        if (context.BoundDisciplineIds.Contains(dto.DisciplineId)) return (null, "Student is already enrolled");

        var discipline = await _context.AddDisciplines
            .AsNoTracking() // Ми просто перевіряємо, відстеження не потрібне
            .FirstOrDefaultAsync(d => d.IdAddDisciplines == dto.DisciplineId);

        if (discipline == null) return (null, "Discipline not found");
        if (!DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context))
            return (null, "Discipline is not available for this student");

        var bind = new BindAddDiscipline
        {
            StudentId = dto.StudentId,
            AddDisciplinesId = dto.DisciplineId,
            Semestr = targetSemester,
            InProcess = 1,
            Loans = dto.Loans // Змінив з хардкоду 5 на dto.Loans, оскільки поле є в DTO!
        };

        _context.BindAddDisciplines.Add(bind);
        await _context.SaveChangesAsync();

        return (bind.IdBindAddDisciplines, null);
    }

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(int id)
    {
        // 3. ВИДАЛЕННЯ INCLUDE ТА ВАЖКИХ ЗАПИТІВ
        // Робимо мапінг до виконання SQL, щоб БД витягнула лише необхідне
        // Якщо у тебе налаштований AutoMapper для IQueryable, використай ProjectTo.
        // Оскільки я не бачу твоїх профілів, пишу надійний Select:

        var dto = await _context.AddDisciplines
            .Where(d => d.IdAddDisciplines == id)
            .Select(d => new FullDisciplineWithDetailsDto
            {
                IdAddDisciplines = d.IdAddDisciplines,
                NameAddDisciplines = d.NameAddDisciplines,
                CodeAddDisciplines = d.CodeAddDisciplines,
                FacultyAbbreviation = d.Faculty != null ? d.Faculty.Abbreviation : null,
                MinCountPeople = d.MinCountPeople,
                MaxCountPeople = d.MaxCountPeople,
                MinCourse = d.MinCourse,
                MaxCourse = d.MaxCourse,
                AddSemestr = d.AddSemestr,
                DegreeLevelName = d.DegreeLevel != null ? d.DegreeLevel.NameEducationalDegreec : "",
                DepartmentName = (d.AddDetail != null && d.AddDetail.Department != null) ? d.AddDetail.Department.NameDepartment : "",
                Teacher = d.AddDetail != null ? d.AddDetail.Teachers : null,
                Recomend = d.AddDetail != null ? d.AddDetail.Recomend : null,
                Prerequisites = d.AddDetail != null ? d.AddDetail.Prerequisites : null,
                Language = d.AddDetail != null ? d.AddDetail.Language : null,
                Determination = d.AddDetail != null ? d.AddDetail.Determination : null,
                WhyInterestingDetermination = d.AddDetail != null ? d.AddDetail.WhyInterestingDetermination : null,
                ResultEducation = d.AddDetail != null ? d.AddDetail.ResultEducation : null,
                UsingIrl = d.AddDetail != null ? d.AddDetail.UsingIrl : null,
                AdditionaLiterature = d.AddDetail != null ? d.AddDetail.AdditionaLiterature : null,
                TypesOfTraining = d.AddDetail != null ? d.AddDetail.TypesOfTraining : "",
                TypeOfControll = d.AddDetail != null ? d.AddDetail.TypeOfControll : ""
            })
            .FirstOrDefaultAsync();

        return dto;
    }

    public async Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateAddDisciplineWithDetailsDto dto)
    {
        // 4. ТРАНЗАКЦІЇ ТА RE-FETCH НЕ ПОТРІБНІ
        // Entity Framework Core автоматично загортає один SaveChangesAsync() у транзакцію.
        // Тобі не потрібно вручну створювати BeginTransactionAsync, якщо ти викликаєш збереження один раз!

        var discipline = _mapper.Map<AddDiscipline>(dto, opts => opts.Items["DbContext"] = _context);
        var details = _mapper.Map<AddDetail>(dto.Details);

        // Магія EF Core: просто присвоюємо об'єкт у навігаційну властивість. 
        // EF сам зрозуміє, що треба спочатку створити Discipline, взяти її ID, 
        // і підставити в Details перед їхнім збереженням.
        discipline.AddDetail = details;

        _context.AddDisciplines.Add(discipline);
        await _context.SaveChangesAsync(); // Атомарна операція, транзакція під капотом

        // Замість того, щоб знову робити запит в БД (re-fetch), 
        // ми просто мапимо те, що щойно створили в пам'яті!
        return _mapper.Map<FullDisciplineWithDetailsDto>((discipline, details));
    }

    public async Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(int id, UpdateAddDisciplineWithDetailsDto dto)
    {
        // Знову ж таки, транзакції тут зайві, якщо в нас один виклик SaveChangesAsync
        var discipline = await _context.AddDisciplines
            .Include(d => d.AddDetail)
            .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

        if (discipline == null) return (false, "Discipline not found");

        if (dto.Details.DepartmentId.HasValue)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.IdDepartment == dto.Details.DepartmentId.Value);
            if (!departmentExists)
                return (false, $"Department with ID {dto.Details.DepartmentId.Value} does not exist");
        }

        if (discipline.AddDetail == null)
        {
            discipline.AddDetail = new AddDetail { IdAddDetails = discipline.IdAddDisciplines };
            _context.AddDetails.Add(discipline.AddDetail);
        }

        _mapper.Map(dto, discipline, opts => opts.Items["DbContext"] = _context);
        _mapper.Map(dto.Details, discipline.AddDetail);

        await _context.SaveChangesAsync(); // Автоматична транзакція!

        return (true, null);
    }
}