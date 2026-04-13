using Azure;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;
using System.Reflection;

namespace OlimpBack.Application.Services;

public class DisciplineTabAdminService : IDisciplineTabAdminService
{
    private readonly IDisciplineTabAdminRepository _repository;
    private readonly IAdminDisciplineStudentListRepository _studentListRepository;

    public DisciplineTabAdminService(
        IDisciplineTabAdminRepository repository,
        IAdminDisciplineStudentListRepository studentListRepository)
    {
        _repository = repository;
        _studentListRepository = studentListRepository;
    }

    public async Task<PaginatedResponseDto<FullDisciplineDto>> GetAllDisciplinesAsync(GetAllDisciplinesAdminQueryDto queryDto)
    {
        var (totalCount, items) = await _repository.GetAllDisciplinesPagedAsync(queryDto);

        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        return new PaginatedResponseDto<FullDisciplineDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items
        };
    }

    private static int GetRequiredCountForSemester(EducationalProgram? program, int semester)
    {
        if (program == null) return 0;
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
        DateTime? periodStart = null;
        if (queryDto.IsNew == 1 && queryDto.FacultyId > 0)
        {
            periodStart = await _repository.GetLastPeriodStartDateAsync(queryDto.FacultyId);
        }

        var studentsData = await _repository.GetStudentsChoicesDataAsync(queryDto, periodStart);
        var items = new List<StudentWithDisciplineChoicesDto>(studentsData.Count);

        foreach (var s in studentsData)
        {
            var selectionOk = true;
            if (s.Program != null)
            {
                for (int sem = 3; sem <= 8; sem++)
                {
                    var required = GetRequiredCountForSemester(s.Program, sem);
                    if (required > 0 && s.SelectedDisciplines.Count(d => d.Semestr == sem) < required)
                    {
                        selectionOk = false;
                        break;
                    }
                }
            }
            else
            {
                selectionOk = false;
            }

            var confirmationOk = s.SelectedDisciplines.Count == 0 || s.SelectedDisciplines.All(d => d.InProcess == 0);

            items.Add(new StudentWithDisciplineChoicesDto
            {
                StudentId = s.IdStudent,
                FullName = s.NameStudent,
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

        var totalItems = items.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);
        var paginated = items.Skip((queryDto.Page - 1) * queryDto.PageSize).Take(queryDto.PageSize).ToList();

        return new PaginatedResponseDto<StudentWithDisciplineChoicesDto>
        {
            TotalPages = totalPages,
            TotalItems = totalItems,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = paginated
        };
    }

    public async Task<UpdateChoiceResponseDto> UpdateChoiceAsync(ConfirmOrRejectChoiceDto[] items)
    {
        var response = new UpdateChoiceResponseDto();
        if (items == null || items.Length == 0) return response;

        var bindIds = items.Select(i => i.BindId).Distinct().ToList();
        var bindsDictionary = await _repository.GetBindsWithDetailsAsync(bindIds);

        var pendingNotifications = new Dictionary<int, Notification>();
        var successfulConfirms = new List<ChoiceResultDto>();
        var successfulRejects = new List<ChoiceResultDto>();

        foreach (var dto in items)
        {
            if (!bindsDictionary.TryGetValue(dto.BindId, out var bind))
            {
                response.Errors.Add(new ChoiceErrorDto { BindId = dto.BindId, Error = "Bind not found" });
                continue;
            }

            if (dto.IsConfirm == 1)
            {
                bind.InProcess = 0;
                successfulConfirms.Add(new ChoiceResultDto
                {
                    Message = "Choice confirmed",
                    BindId = bind.IdBindAddDisciplines,
                    DisciplineName = bind.AddDisciplines?.NameAddDisciplines
                });
            }
            else if (dto.IsConfirm == 0)
            {
                var userId = bind.Student?.UserId;
                
                if (userId == null || userId <= 0)
                {
                    response.Errors.Add(new ChoiceErrorDto { BindId = dto.BindId, Error = "Student has no valid UserId for notification." });
                    continue;
                }

                var (success, errorMessage) = await RepealChoiceAsync(bind.AddDisciplinesId, (int)bind.Student?.IdStudent);
                if (!success)
                {
                    response.Errors.Add(new ChoiceErrorDto { BindId = dto.BindId, Error = errorMessage ?? "Failed to repeal choice" });
                    continue;
                }
                /*
                var disciplineName = bind.AddDisciplines?.NameAddDisciplines ?? "elective";
                _repository.RemoveBind(bind);

                var notification = new Notification
                {
                    UserId = userId.Value,
                    CustomTitle = "Elective discipline rejected",
                    CustomMessage = $"Your choice \"{disciplineName}\" was rejected by the administrator.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    NotificationType = "DisciplineRejected"
                };

                _repository.AddNotification(notification);
                pendingNotifications.Add(dto.BindId, notification);

                successfulRejects.Add(new ChoiceResultDto
                {
                    Message = "Choice rejected and student notified",
                    BindId = dto.BindId,
                    DisciplineName = disciplineName
                });*/
            }
            else
            {
                response.Errors.Add(new ChoiceErrorDto { BindId = dto.BindId, Error = "Action must be 0 (Reject) or 1 (Confirm)" });
            }
        }

        if (successfulConfirms.Any() || pendingNotifications.Any())
            await _repository.SaveChangesAsync();

        response.Results.AddRange(successfulConfirms);
        foreach (var rejectResult in successfulRejects)
        {
            if (pendingNotifications.TryGetValue(rejectResult.BindId, out var savedNotification))
                rejectResult.NotificationId = savedNotification.IdNotification;
            response.Results.Add(rejectResult);
        }

        if (!response.Errors.Any()) response.Errors = null!;
        return response;
    }


    public async Task<PaginatedResponseDto<AdminDisciplineListItemDto>> GetDisciplinesWithStatusAsync(GetDisciplinesWithStatusQueryDto queryDto)
    {
        var disciplinesData = await _repository.GetDisciplinesStatusDataAsync(queryDto);
        var lastPeriodByFaculty = await _repository.GetLastPeriodsByFacultyAsync();
        var normativeLookup = await _repository.GetNormativesLookupAsync();

        var fullList = new List<AdminDisciplineListItemDto>(disciplinesData.Count);

        foreach (var d in disciplinesData)
        {
            var periodStart = lastPeriodByFaculty.TryGetValue(d.FacultyId, out var start) ? start : DateTime.MinValue;
            var currentCount = d.BindDates.Count(date => date >= periodStart);

            var normativeCount = d.DegreeLevelId.HasValue && normativeLookup.TryGetValue((d.DegreeLevelId.Value, d.IsFaculty), out var norm) ? norm : (int?)null;

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
                Credits = 5,
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

        if (queryDto.StatusFilter.HasValue)
        {
            var statusDictionary = new Dictionary<int, string> { { 1, "Not Acquired" }, { 2, "Smartly Acquired" }, { 3, "Accepted" }, { 4, "Collected" } };
            if (statusDictionary.TryGetValue(queryDto.StatusFilter.Value, out var filterStatus))
                fullList = fullList.Where(x => x.Status == filterStatus).ToList();
        }

        fullList = queryDto.SortOrder switch
        {
            1 => fullList.OrderByDescending(d => d.NameAddDisciplines).ToList(),
            2 => fullList.OrderBy(d => d.CurrentCount).ToList(),
            3 => fullList.OrderByDescending(d => d.CurrentCount).ToList(),
            _ => fullList.OrderBy(d => d.NameAddDisciplines).ToList()
        };

        var totalItems = fullList.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);
        var paginated = fullList.Skip((queryDto.Page - 1) * queryDto.PageSize).Take(queryDto.PageSize).ToList();

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
        var statusDictionary = new Dictionary<int, string> { { 1, "Not Selected" }, { 2, "Intellectually Selected" }, { 3, "Selected" }, { 4, "Collected" } };
        if (!statusDictionary.TryGetValue(dto.Status, out var statusName))
            throw new ArgumentException($"Invalid status value: {dto.Status}. Must be between 1 and 4.");

        var discipline = await _repository.GetDisciplineEntityAsync(dto.DisciplineId);
        if (discipline == null) return null;

        discipline.IsForseChange = 1;
        discipline.TypeId = dto.Status;
        await _repository.SaveChangesAsync();

        return new UpdateDisciplineStatusResponseDto { Message = "Discipline status updated", DisciplineId = discipline.IdAddDisciplines, Status = statusName, IsForceChange = 1 };
    }

    public async Task<BindAddDisciplineDto?> GetBindAsync(int id) =>
        await _repository.GetBindDtoAsync(id);

    public async Task<StudentWithDisciplineChoicesDto?> GetStudentWithChoicesAsync(int studentId)
    {
        var studentData = await _repository.GetStudentChoicesDataAsync(studentId);
        if (studentData == null) return null;

        var selectionOk = true;
        if (studentData.Program != null)
        {
            for (int sem = 3; sem <= 8; sem++)
            {
                var required = GetRequiredCountForSemester(studentData.Program, sem);
                if (required > 0 && studentData.SelectedDisciplines.Count(d => d.Semestr == sem) < required)
                {
                    selectionOk = false;
                    break;
                }
            }
        }
        else { selectionOk = false; }

        var confirmationOk = studentData.SelectedDisciplines.Count == 0 || studentData.SelectedDisciplines.All(d => d.InProcess == 0);

        return new StudentWithDisciplineChoicesDto
        {
            StudentId = studentData.IdStudent,
            FullName = studentData.NameStudent,
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
        if (dto.Semestr < 1 || dto.Semestr > 8) return (null, "Semestr must be between 1 and 8");
        if (await _repository.ExistsBindAsync(dto.StudentId, dto.DisciplineId)) return (null, "This student is already bound to this discipline");
        if (!await _repository.ExistsStudentAsync(dto.StudentId)) return (null, "Student not found");
        if (!await _repository.ExistsDisciplineAsync(dto.DisciplineId)) return (null, "Discipline not found");

        var bind = new BindAddDiscipline { StudentId = dto.StudentId, AddDisciplinesId = dto.DisciplineId, Semestr = dto.Semestr, Loans = dto.Loans, InProcess = 1 };
        await _repository.AddBindAsync(bind);
        await _repository.SaveChangesAsync();

        return (bind.IdBindAddDisciplines, null);
    }

    public async Task<(bool success, string? errorMessage)> RepealChoiceAsync(int disciplineId, int studentId)
    {
        // 1. Шукаємо зв'язок за двома ID
        var bind = await _repository.GetBindByStudentAndDisciplineAsync(studentId, disciplineId);

        if (bind == null)
            return (false, "Choice bind not found for this student and discipline.");

        var userId = bind.Student?.UserId;
        if (userId == null || userId <= 0)
            return (false, "Student has no valid UserId for notification.");

        var disciplineName = bind.AddDisciplines?.NameAddDisciplines ?? "elective";

        // 2. Видаляємо
        _repository.RemoveBind(bind);

        // 3. Відправляємо сповіщення
        var notification = new Notification
        {
            UserId = userId.Value,
            CustomTitle = "Elective discipline rejected",
            CustomMessage = $"Your choice \"{disciplineName}\" was rejected by the administrator.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            NotificationType = "DisciplineRejected"
        };

        _repository.AddNotification(notification);

        // 4. Зберігаємо (бо це одинична дія)
        await _repository.SaveChangesAsync();

        return (true, null);
    }
    public async Task<bool> DeleteBindAsync(int id) =>
        await _repository.DeleteBindAsync(id) > 0;

    public async Task<PaginatedResponseDto<AdminStudentByAddDisciplineDto>> GetStudentsByAddDisciplineAsync(GetStudentsByAddDisciplineQueryDto query)
    {
        var (totalCount, items) = await _studentListRepository.GetStudentsByAddDisciplineAsync(query);
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PaginatedResponseDto<AdminStudentByAddDisciplineDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = query.Page,
            PageSize = query.PageSize,
            Items = items
        };
    }

    public async Task<PaginatedResponseDto<AdminStudentByMainDisciplineDto>> GetStudentsByMainDisciplineAsync(GetStudentsByMainDisciplineQueryDto query)
    {
        var (totalCount, items) = await _studentListRepository.GetStudentsByMainDisciplineAsync(query);
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PaginatedResponseDto<AdminStudentByMainDisciplineDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = query.Page,
            PageSize = query.PageSize,
            Items = items
        };
    }
}