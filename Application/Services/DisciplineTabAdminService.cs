using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class DisciplineTabAdminService : IDisciplineTabAdminService
{
    private readonly AppDbContext _context;

    public DisciplineTabAdminService(AppDbContext context)
    {
        _context = context;
    }

    private static int GetRequiredCountForSemester(EducationalProgram program, int semester)
    {
        return semester switch
        {
            3 => program.CountAddSemestr3 ?? 0,
            4 => program.CountAddSemestr4 ?? 0,
            5 => program.CountAddSemestr5 ?? 0,
            6 => program.CountAddSemestr6 ?? 0,
            7 => program.CountAddSemestr7 ?? 0,
            8 => program.CountAddSemestr8 ?? 0,
            _ => 0
        };
    }


    public async Task<PaginatedResponseDto<StudentWithDisciplineChoicesDto>> GetStudentsWithDisciplineChoicesAsync(GetStudentsWithDisciplineChoicesQueryDto queryDto)
    {
        var query = _context.Students.AsQueryable();

        // 1. ФІЛЬТРАЦІЯ НА РІВНІ БАЗИ ДАНИХ (БЕЗ SPLIT!)
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s =>
                EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%") ||
                s.Faculty != null && (
                    EF.Functions.Like(s.Faculty.NameFaculty.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(s.Faculty.Abbreviation.ToLower(), $"%{lowerSearch}%")) ||
                s.Group != null && EF.Functions.Like(s.Group.GroupCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(s => queryDto.Faculties.Contains(s.FacultyId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(s => queryDto.Courses.Contains(s.Course));

        if (queryDto.Groups != null && queryDto.Groups.Any())
            query = query.Where(s => queryDto.Groups.Contains(s.GroupId));

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(s => queryDto.DegreeLevelIds.Contains(s.EducationalDegreeId));

        DateTime? periodStart = null;
        if (queryDto.IsNew == 1 && queryDto.FacultyId > 0)
        {
            query = query.Where(s => s.FacultyId == queryDto.FacultyId);
            periodStart = await _context.DisciplineChoicePeriods
                .Where(p => p.FacultyId == queryDto.FacultyId)
                .OrderByDescending(p => p.StartDate)
                .Select(p => p.StartDate)
                .FirstOrDefaultAsync();
        }

        // 2. ОПТИМІЗОВАНА ВИБІРКА (ЛЕГКОВІСНА)
        // Замість Include тягнемо тільки потрібні поля в пам'ять. Це зменшить споживання ОЗП в десятки разів.
        var studentsData = await query.Select(s => new
        {
            s.IdStudent,
            s.NameStudent,
            FacultyName = s.Faculty != null ? s.Faculty.Abbreviation ?? s.Faculty.NameFaculty : "",
            GroupCode = s.Group != null ? s.Group.GroupCode : "",
            s.Course,
            s.EducationalDegreeId,
            DegreeName = s.EducationalDegree != null ? s.EducationalDegree.NameEducationalDegreec : "",
            Program = s.EducationalProgram,
            // Одразу мапимо дисципліни, EF Core перетворить це на LEFT JOIN
            SelectedDisciplines = s.BindAddDisciplines
                .Where(b => queryDto.IsNew == 0 || periodStart == null || b.CreatedAt >= periodStart)
                .Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindAddDisciplines = b.IdBindAddDisciplines,
                    IdAddDisciplines = b.AddDisciplinesId,
                    NameAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines : "",
                    CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeAddDisciplines : "",
                    Semestr = b.Semestr,
                    InProcess = b.InProcess
                }).ToList()
        }).ToListAsync();

        // 3. ОБРОБКА БІЗНЕС-ЛОГІКИ В ПАМ'ЯТІ
        var items = new List<StudentWithDisciplineChoicesDto>(studentsData.Count);

        foreach (var s in studentsData)
        {
            var selectionOk = true;

            // ВИПРАВЛЕНИЙ БАГ: Тепер ми перевіряємо всі семестри від 3 до 8.
            // Якщо студент мав обрати, але обрав 0 (немає в колекції), selectionOk стане false!
            if (s.Program != null)
            {
                for (int sem = 3; sem <= 8; sem++)
                {
                    var required = GetRequiredCountForSemester(s.Program, sem);
                    if (required > 0)
                    {
                        var count = s.SelectedDisciplines.Count(d => d.Semestr == sem);
                        if (count < required)
                        {
                            selectionOk = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                selectionOk = false; // Якщо немає програми, вибір не може бути валідним
            }

            var confirmationOk = s.SelectedDisciplines.Count == 0 || s.SelectedDisciplines.All(d => d.InProcess == 0);

            items.Add(new StudentWithDisciplineChoicesDto
            {
                StudentId = s.IdStudent,
                FullName = s.NameStudent ?? "",
                Faculty = s.FacultyName,
                Group = s.GroupCode,
                Year = s.Course,
                DegreeLevelId = s.EducationalDegreeId,
                DegreeLevelName = s.DegreeName,
                SelectedDisciplines = s.SelectedDisciplines,
                SelectionStatus = selectionOk ? 1 : 0,
                ConfirmationStatus = confirmationOk ? 1 : 0
            });
        }

        // 4. ФІЛЬТРАЦІЯ ПО СТАТУСАХ І СОРТУВАННЯ
        if (queryDto.SelectionStatus.HasValue)
            items = items.Where(x => x.SelectionStatus == queryDto.SelectionStatus.Value).ToList();

        if (queryDto.ConfirmationStatus.HasValue)
            items = items.Where(x => x.ConfirmationStatus == queryDto.ConfirmationStatus.Value).ToList();

        items = queryDto.SortOrder switch
        {
            1 => items.OrderByDescending(x => x.FullName).ToList(),
            2 => items.OrderBy(x => x.Faculty).ToList(),
            3 => items.OrderByDescending(x => x.Faculty).ToList(),
            4 => items.OrderBy(x => x.Group).ToList(),
            5 => items.OrderByDescending(x => x.Group).ToList(),
            6 => items.OrderBy(x => x.Year).ToList(),
            7 => items.OrderByDescending(x => x.Year).ToList(),
            _ => items.OrderBy(x => x.FullName).ToList()
        };

        // 5. ПАГІНАЦІЯ
        var totalItems = items.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        var paginated = items
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        return new PaginatedResponseDto<StudentWithDisciplineChoicesDto>
        {
            TotalPages = totalPages,
            TotalItems = totalItems,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = paginated,
            Filters = queryDto // Якщо фронту дуже треба, віддаємо сам об'єкт запиту назад
        };
    }
    
    public async Task<UpdateChoiceResponseDto> UpdateChoiceAsync(ConfirmOrRejectChoiceDto[] items)
    {
        var response = new UpdateChoiceResponseDto();

        if (items == null || items.Length == 0)
            return response;

        // 1. ВИРІШУЄМО ПРОБЛЕМУ N+1
        // Збираємо всі унікальні ID і робимо ЛИШЕ ОДИН запит до бази даних
        var bindIds = items.Select(i => i.BindId).Distinct().ToList();

        var bindsDictionary = await _context.BindAddDisciplines
            .Include(b => b.Student)
            .Include(b => b.AddDisciplines)
            .Where(b => bindIds.Contains(b.IdBindAddDisciplines))
            .ToDictionaryAsync(b => b.IdBindAddDisciplines);

        // Словник для збереження зв'язку між BindId та створеною нотифікацією,
        // щоб після SaveChanges() дістати згенерований NotificationId
        var pendingNotifications = new Dictionary<int, Notification>();
        var successfulConfirms = new List<ChoiceResultDto>();
        var successfulRejects = new List<ChoiceResultDto>();

        // 2. ОБРОБКА ДАНИХ У ПАМ'ЯТІ (БЕЗ ЗАПИТІВ ДО БД)
        foreach (var dto in items)
        {
            if (!bindsDictionary.TryGetValue(dto.BindId, out var bind))
            {
                response.Errors.Add(new ChoiceErrorDto { BindId = dto.BindId, Error = "Bind not found" });
                continue;
            }

            if (dto.IsConfirm == 1) // Підтвердження
            {
                bind.InProcess = 0;
                successfulConfirms.Add(new ChoiceResultDto
                {
                    Message = "Choice confirmed",
                    BindId = bind.IdBindAddDisciplines,
                    DisciplineName = bind.AddDisciplines?.NameAddDisciplines
                });
            }
            else if (dto.IsConfirm == 0) // Відхилення
            {
                // БЕЗПЕКА: Перевіряємо наявність UserId, щоб база не впала з помилкою Foreign Key
                var userId = bind.Student?.UserId;
                if (userId == null || userId <= 0)
                {
                    response.Errors.Add(new ChoiceErrorDto
                    {
                        BindId = dto.BindId,
                        Error = "Student has no valid UserId for notification. Rejection aborted."
                    });
                    continue;
                }

                var disciplineName = bind.AddDisciplines?.NameAddDisciplines ?? "elective";

                _context.BindAddDisciplines.Remove(bind);

                var notification = new Notification
                {
                    UserId = userId.Value,
                    CustomTitle = "Elective discipline rejected",
                    CustomMessage = $"Your choice \"{disciplineName}\" was rejected by the administrator.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    NotificationType = "DisciplineRejected"
                };

                _context.Notifications.Add(notification);
                pendingNotifications.Add(dto.BindId, notification); // Зберігаємо посилання на об'єкт

                successfulRejects.Add(new ChoiceResultDto
                {
                    Message = "Choice rejected and student notified",
                    BindId = dto.BindId,
                    DisciplineName = disciplineName
                });
            }
            else
            {
                response.Errors.Add(new ChoiceErrorDto { BindId = dto.BindId, Error = "Action must be 0 (Reject) or 1 (Confirm)" });
            }
        }

        // 3. ЗБЕРІГАЄМО ВСІ ЗМІНИ ОДНИМ ТРАНЗАКЦІЙНИМ ВИКЛИКОМ
        // Тільки зараз EF Core відправить SQL-запити в БД і згенерує ID для нотифікацій
        if (successfulConfirms.Any() || pendingNotifications.Any())
        {
            await _context.SaveChangesAsync();
        }

        // 4. ДОДАЄМО РЕЗУЛЬТАТИ ТА ПРИСВОЮЄМО ЗГЕНЕРОВАНІ ID
        response.Results.AddRange(successfulConfirms);

        foreach (var rejectResult in successfulRejects)
        {
            if (pendingNotifications.TryGetValue(rejectResult.BindId, out var savedNotification))
            {
                // Тепер savedNotification.IdNotification містить справжній ID з бази!
                rejectResult.NotificationId = savedNotification.IdNotification;
            }
            response.Results.Add(rejectResult);
        }

        // Якщо помилок немає, можна повернути null замість порожнього масиву, якщо так вимагає твій фронтенд
        if (!response.Errors.Any()) response.Errors = null!;

        return response;
    }
    
    public async Task<PaginatedResponseDto<AdminDisciplineListItemDto>> GetDisciplinesWithStatusAsync(GetDisciplinesWithStatusQueryDto queryDto)
    {
        var query = _context.AddDisciplines.AsNoTracking().AsQueryable();

        // 1. ФІЛЬТРАЦІЯ НА РІВНІ БАЗИ ДАНИХ
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(d =>
                EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
            query = query.Where(d => queryDto.Faculties.Contains(d.FacultyId));

        if (queryDto.IsFaculty.HasValue)
            query = query.Where(d => d.IsFaculty == queryDto.IsFaculty.Value);

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(d => d.DegreeLevelId.HasValue && queryDto.DegreeLevelIds.Contains(d.DegreeLevelId.Value));

        // 2. ПОПЕРЕДНЄ ЗАВАНТАЖЕННЯ ДОВІДНИКІВ (Вони маленькі, тому тягнемо в пам'ять)
        var lastPeriodByFaculty = await _context.DisciplineChoicePeriods
            .Where(p => p.FacultyId != null)
            .GroupBy(p => p.FacultyId!.Value)
            .Select(g => new { FacultyId = g.Key, StartDate = g.Max(p => p.StartDate) })
            .ToDictionaryAsync(x => x.FacultyId, x => x.StartDate);

        var normativeLookup = await _context.Normatives
            .Where(n => n.DegreeLevelId != null)
            .GroupBy(n => new { Level = n.DegreeLevelId!.Value, n.IsFaculty })
            .ToDictionaryAsync(g => g.Key, g => g.First().Count);

        // 3. ЛЕГКОВАГОВА ПРОЕКЦІЯ (Тягнемо мінімум даних замість Include)
        var disciplinesData = await query.Select(d => new
        {
            d.IdAddDisciplines,
            d.NameAddDisciplines,
            Teachers = d.AddDetail != null ? d.AddDetail.Teachers : null,
            DepartmentName = d.AddDetail != null && d.AddDetail.Department != null ? d.AddDetail.Department.NameDepartment : null,
            d.MinCountPeople,
            d.MaxCountPeople,
            d.IsForseChange, // Зберіг твоє написання з 's'
            TypeName = d.Type != null ? d.Type.TypeName : "",
            d.DegreeLevelId,
            d.IsFaculty,
            d.FacultyId,
            FacultyAbbreviation = d.Faculty != null ? d.Faculty.Abbreviation : null,
            // Замість всіх об'єктів заявок тягнемо ЛИШЕ їхні дати створення!
            BindDates = d.BindAddDisciplines.Select(b => b.CreatedAt).ToList()
        }).ToListAsync();

        // 4. ОБРОБКА БІЗНЕС-ЛОГІКИ В ПАМ'ЯТІ
        var fullList = new List<AdminDisciplineListItemDto>(disciplinesData.Count);

        foreach (var d in disciplinesData)
        {
            // Рахуємо кількість актуальних заявок
            var periodStart = lastPeriodByFaculty.TryGetValue(d.FacultyId, out var start) ? start : DateTime.MinValue;
            var currentCount = d.BindDates.Count(date => date >= periodStart);

            // Отримуємо норматив
            var lookupKey = new { Level = d.DegreeLevelId ?? 0, d.IsFaculty };
            var normativeCount = d.DegreeLevelId.HasValue && normativeLookup.TryGetValue(lookupKey, out var norm)
                ? norm
                : (int?)null;

            // Визначаємо статус
            string statusStr;
            if (d.IsForseChange == 1)
            {
                statusStr = string.IsNullOrEmpty(d.TypeName) ? string.Empty : d.TypeName;
            }
            else if (normativeCount == null || normativeCount == 0)
            {
                statusStr = currentCount >= (d.MinCountPeople ?? 0) ? "Accepted" : "Not Acquired";
            }
            else
            {
                var ratio = (double)currentCount / normativeCount.Value;
                statusStr = ratio >= 1.0 ? "Accepted" : ratio >= 0.8 ? "Smartly Acquired" : "Not Acquired";
            }

            fullList.Add(new AdminDisciplineListItemDto
            {
                IdAddDisciplines = d.IdAddDisciplines,
                NameAddDisciplines = d.NameAddDisciplines,
                Teachers = d.Teachers,
                DepartmentName = d.DepartmentName,
                Credits = 5, // Твоє хардкод значення
                Normative = normativeCount,
                MaxCountPeople = d.MaxCountPeople,
                CurrentCount = currentCount,
                Status = statusStr,
                IsForceChange = d.IsForseChange,
                DegreeLevelId = d.DegreeLevelId,
                IsFaculty = d.IsFaculty,
                FacultyId = d.FacultyId,
                FacultyAbbreviation = d.FacultyAbbreviation
            });
        }

        // 5. ФІЛЬТРАЦІЯ ПО СТАТУСУ (Безпечний мапінг через словник)
        if (queryDto.StatusFilter.HasValue)
        {
            var statusDictionary = new Dictionary<int, string>
        {
            { 1, "Not Acquired" },
            { 2, "Smartly Acquired" },
            { 3, "Accepted" },
            { 4, "Collected" }
        };

            if (statusDictionary.TryGetValue(queryDto.StatusFilter.Value, out var filterStatus))
            {
                fullList = fullList.Where(x => x.Status == filterStatus).ToList();
            }
        }

        // 6. СОРТУВАННЯ ТА ПАГІНАЦІЯ
        fullList = queryDto.SortOrder switch
        {
            1 => fullList.OrderByDescending(d => d.NameAddDisciplines).ToList(),
            2 => fullList.OrderBy(d => d.CurrentCount).ToList(),
            3 => fullList.OrderByDescending(d => d.CurrentCount).ToList(),
            _ => fullList.OrderBy(d => d.NameAddDisciplines).ToList()
        };

        var totalItems = fullList.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        var paginated = fullList
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        // Використовуємо наш PagedResponse<T> з попереднього кроку
        return new PaginatedResponseDto<AdminDisciplineListItemDto>
        {
            TotalPages = totalPages,
            TotalItems = totalItems,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = paginated
        };
    }
    
    public async Task<UpdateDisciplineStatusResponseDto?> UpdateDisciplineStatusAsync(UpdateDisciplineStatusDto dto)
    {
        // 1. БЕЗПЕЧНИЙ СЛОВНИК ЗАМІСТЬ МАСИВУ
        // Відкидаємо нульовий "Empty", бо в тебе статуси в DTO йдуть від 1 до 4
        var statusDictionary = new Dictionary<int, string>
        {
            { 1, "Not Selected" }, // Зверни увагу: в коментарях DTO ти писав "1 = Not Selected"
            { 2, "Intellectually Selected" }, // "2 = Intellectually Selected" (Smartly Acquired)
            { 3, "Selected" }, // "3 = Selected" (Accepted)
            { 4, "Collected" }
        };

        // 2. ВАЛІДАЦІЯ ВХІДНИХ ДАНИХ
        if (!statusDictionary.TryGetValue(dto.Status, out var statusName))
        {
            // Якщо прийшов невалідний статус, можна викинути помилку або обробити це.
            // Оскільки метод повертає null при помилці пошуку, викинемо ArgumentException
            throw new ArgumentException($"Invalid status value: {dto.Status}. Must be between 1 and 4.");
        }

        // 3. ШУКАЄМО СУТНІСТЬ
        var discipline = await _context.AddDisciplines.FindAsync(dto.DisciplineId);
        if (discipline == null)
            return null; // Контролер має перехопити це і повернути NotFound()

        // 4. ОНОВЛЮЄМО ДАНІ
        discipline.IsForseChange = 1; // Залишив твоє написання з 's'
        discipline.TypeId = dto.Status;

        await _context.SaveChangesAsync();

        // 5. ПОВЕРТАЄМО ЧІТКИЙ DTO
        return new UpdateDisciplineStatusResponseDto
        {
            Message = "Discipline status updated",
            DisciplineId = discipline.IdAddDisciplines,
            Status = statusName,
            IsForceChange = 1
        };
    }
    public async Task<BindAddDisciplineDto?> GetBindAsync(int id)
    {
        // Відразу робимо проекцію (Select). Include більше не потрібен!
        var bindDto = await _context.BindAddDisciplines
            .Where(b => b.IdBindAddDisciplines == id)
            .Select(b => new BindAddDisciplineDto
            {
                IdBindAddDisciplines = b.IdBindAddDisciplines,
                StudentId = b.StudentId,
                // EF Core сам зробить JOIN і безпечно витягне NameStudent
                StudentFullName = b.Student != null ? b.Student.NameStudent : "",
                AddDisciplinesId = b.AddDisciplinesId,
                AddDisciplineName = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines : "",
                Semestr = b.Semestr,
                Loans = b.Loans,
                InProcess = b.InProcess == 1
            })
            .FirstOrDefaultAsync();

        // Якщо запис не знайдено, bindDto буде null. Це саме те, що нам треба!
        return bindDto;
    }

    public async Task<StudentWithDisciplineChoicesDto?> GetStudentWithChoicesAsync(int studentId)
    {
        // 1. ОПТИМІЗОВАНИЙ ЗАПИТ (Без Include)
        var studentData = await _context.Students
            .Where(s => s.IdStudent == studentId)
            .Select(s => new
            {
                s.IdStudent,
                s.NameStudent,
                FacultyName = s.Faculty != null ? s.Faculty.Abbreviation ?? s.Faculty.NameFaculty : "",
                GroupCode = s.Group != null ? s.Group.GroupCode : "",
                s.Course,
                s.EducationalDegreeId,
                DegreeName = s.EducationalDegree != null ? s.EducationalDegree.NameEducationalDegreec : "",
                Program = s.EducationalProgram,
                SelectedDisciplines = s.BindAddDisciplines.Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindAddDisciplines = b.IdBindAddDisciplines,
                    IdAddDisciplines = b.AddDisciplinesId,
                    NameAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.NameAddDisciplines : "",
                    CodeAddDisciplines = b.AddDisciplines != null ? b.AddDisciplines.CodeAddDisciplines : "",
                    Semestr = b.Semestr,
                    InProcess = b.InProcess
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (studentData == null)
            return null;

        // 2. ВИПРАВЛЕНА БІЗНЕС-ЛОГІКА ПЕРЕВІРКИ
        var selectionOk = true;

        if (studentData.Program != null)
        {
            // Перевіряємо всі семестри, де МОЖЕ бути вибір (3-8)
            for (int sem = 3; sem <= 8; sem++)
            {
                var required = GetRequiredCountForSemester(studentData.Program, sem);
                if (required > 0)
                {
                    var count = studentData.SelectedDisciplines.Count(d => d.Semestr == sem);
                    if (count < required)
                    {
                        selectionOk = false;
                        break; // Якщо хоча б в одному семестрі недобір — статус 0
                    }
                }
            }
        }
        else
        {
            selectionOk = false; // Якщо немає програми, вибір не може бути валідним
        }

        var confirmationOk = studentData.SelectedDisciplines.Count == 0 ||
                             studentData.SelectedDisciplines.All(d => d.InProcess == 0);

        // 3. ПОВЕРНЕННЯ ЧІТКОГО DTO
        return new StudentWithDisciplineChoicesDto
        {
            StudentId = studentData.IdStudent,
            FullName = studentData.NameStudent ?? "",
            Faculty = studentData.FacultyName,
            Group = studentData.GroupCode,
            Year = studentData.Course,
            DegreeLevelId = studentData.EducationalDegreeId,
            DegreeLevelName = studentData.DegreeName,
            SelectedDisciplines = studentData.SelectedDisciplines,
            SelectionStatus = selectionOk ? 1 : 0,
            ConfirmationStatus = confirmationOk ? 1 : 0
        };
    }

    public async Task<(int? bindId, string? error)> CreateBindAsync(AddDisciplineBindDto dto)
    {
        // 1. Швидка валідація БЕЗ походів у БД (Fail Fast)
        if (dto.Semestr < 1 || dto.Semestr > 8)
            return (null, "Semestr must be between 1 and 8");

        // 2. Перевірка на дублікат (найчастіша помилка, тому перевіряємо відразу)
        var exists = await _context.BindAddDisciplines
            .AnyAsync(b => b.StudentId == dto.StudentId && b.AddDisciplinesId == dto.DisciplineId);
        if (exists)
            return (null, "This student is already bound to this discipline");

        // 3. Перевірка існування студента і дисципліни через AnyAsync (дуже швидкі запити EXISTS у SQL)
        var studentExists = await _context.Students.AnyAsync(s => s.IdStudent == dto.StudentId);
        if (!studentExists)
            return (null, "Student not found");

        var disciplineExists = await _context.AddDisciplines.AnyAsync(d => d.IdAddDisciplines == dto.DisciplineId);
        if (!disciplineExists)
            return (null, "Discipline not found");

        // 4. Створення запису
        var bind = new BindAddDiscipline
        {
            StudentId = dto.StudentId,
            AddDisciplinesId = dto.DisciplineId,
            Semestr = dto.Semestr,
            Loans = dto.Loans,
            InProcess = 1
        };

        _context.BindAddDisciplines.Add(bind);
        await _context.SaveChangesAsync();

        return (bind.IdBindAddDisciplines, null);
    }

    public async Task<bool> DeleteBindAsync(int id)
    {
        // Робить прямий DELETE FROM BindAddDisciplines WHERE IdBindAddDisciplines = id
        var deletedCount = await _context.BindAddDisciplines
            .Where(b => b.IdBindAddDisciplines == id)
            .ExecuteDeleteAsync();

        return deletedCount > 0;
    }
}
