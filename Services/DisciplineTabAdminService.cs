using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

namespace OlimpBack.Services;

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

    public async Task<object?> GetStudentsWithDisciplineChoicesAsync(GetStudentsWithDisciplineChoicesQueryDto queryDto)
    {
        var query = _context.Students
            .Include(s => s.Faculty)
            .Include(s => s.Group)
            .Include(s => s.EducationalDegree)
            .Include(s => s.EducationalProgram)
            .Include(s => s.BindAddDisciplines)
                .ThenInclude(b => b.AddDisciplines)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s =>
                EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%") ||
                (s.Faculty != null && (
                    EF.Functions.Like(s.Faculty.NameFaculty.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(s.Faculty.Abbreviation.ToLower(), $"%{lowerSearch}%"))) ||
                (s.Group != null && EF.Functions.Like(s.Group.GroupCode.ToLower(), $"%{lowerSearch}%")));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Faculties))
        {
            var facultyIds = queryDto.Faculties
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(f => int.TryParse(f.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
            if (facultyIds.Any())
                query = query.Where(s => facultyIds.Contains(s.FacultyId));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Courses))
        {
            var courseList = queryDto.Courses
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => int.TryParse(c.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
            if (courseList.Any())
                query = query.Where(s => courseList.Contains(s.Course));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Groups))
        {
            var groupIds = queryDto.Groups
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(g => int.TryParse(g.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
            if (groupIds.Any())
                query = query.Where(s => groupIds.Contains(s.GroupId));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds))
        {
            var levelIds = queryDto.DegreeLevelIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => int.TryParse(d.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
            if (levelIds.Any())
                query = query.Where(s => levelIds.Contains(s.EducationalDegreeId));
        }

        DateTime? periodStart = null;
        if (queryDto.IsNew == 1 && queryDto.FacultyId > 0)
        {
            query = query.Where(s => s.FacultyId == queryDto.FacultyId);
            var lastPeriod = await _context.DisciplineChoicePeriods
                .Where(p => p.FacultyId == queryDto.FacultyId)
                .OrderByDescending(p => p.StartDate)
                .Select(p => new { p.StartDate })
                .FirstOrDefaultAsync();
            periodStart = lastPeriod?.StartDate;
        }

        var students = await query.ToListAsync();

        var items = new List<StudentWithDisciplineChoicesDto>();
        foreach (var s in students)
        {
            var bindSource = s.BindAddDisciplines.AsEnumerable();
            if (queryDto.IsNew == 1 && periodStart.HasValue)
                bindSource = bindSource.Where(b => b.CreatedAt >= periodStart.Value);

            var selected = bindSource
                .Select(b => new StudentSelectedDisciplineDto
                {
                    IdBindAddDisciplines = b.IdBindAddDisciplines,
                    IdAddDisciplines = b.AddDisciplinesId,
                    NameAddDisciplines = b.AddDisciplines?.NameAddDisciplines ?? "",
                    CodeAddDisciplines = b.AddDisciplines?.CodeAddDisciplines ?? "",
                    Semestr = b.Semestr,
                    InProcess = b.InProcess
                })
                .ToList();

            var semestersWithChoices = selected.Select(x => x.Semestr).Distinct().ToList();
            var selectionOk = true;
            if (s.EducationalProgram != null)
            {
                foreach (var sem in semestersWithChoices)
                {
                    var required = GetRequiredCountForSemester(s.EducationalProgram, sem);
                    var count = selected.Count(d => d.Semestr == sem);
                    if (count < required)
                    {
                        selectionOk = false;
                        break;
                    }
                }
            }

            var confirmationOk = selected.Count == 0 || selected.All(d => d.InProcess == 0);

            items.Add(new StudentWithDisciplineChoicesDto
            {
                StudentId = s.IdStudent,
                FullName = s.NameStudent ?? "",
                Faculty = s.Faculty?.Abbreviation ?? s.Faculty?.NameFaculty ?? "",
                Group = s.Group?.GroupCode ?? "",
                Year = s.Course,
                DegreeLevelId = s.EducationalDegreeId,
                DegreeLevelName = s.EducationalDegree?.NameEducationalDegreec ?? "",
                SelectedDisciplines = selected,
                SelectionStatus = selectionOk ? 1 : 0,
                ConfirmationStatus = confirmationOk ? 1 : 0
            });
        }

        if (queryDto.SelectionStatus.HasValue && (queryDto.SelectionStatus.Value == 0 || queryDto.SelectionStatus.Value == 1))
            items = items.Where(x => x.SelectionStatus == queryDto.SelectionStatus.Value).ToList();

        if (queryDto.ConfirmationStatus.HasValue && (queryDto.ConfirmationStatus.Value == 0 || queryDto.ConfirmationStatus.Value == 1))
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
        var paginated = items
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        return new
        {
            totalPages,
            totalItems,
            currentPage = queryDto.Page,
            pageSize = queryDto.PageSize,
            students = paginated,
            filters = new
            {
                faculties = string.IsNullOrWhiteSpace(queryDto.Faculties) ? null : queryDto.Faculties.Split(',').Select(f => f.Trim()).ToList(),
                courses = string.IsNullOrWhiteSpace(queryDto.Courses) ? null : queryDto.Courses.Split(',').Select(c => c.Trim()).ToList(),
                groups = string.IsNullOrWhiteSpace(queryDto.Groups) ? null : queryDto.Groups.Split(',').Select(g => g.Trim()).ToList(),
                degreeLevelIds = string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds) ? null : queryDto.DegreeLevelIds.Split(',').Select(d => int.Parse(d.Trim())).ToList(),
                selectionStatus = queryDto.SelectionStatus,
                confirmationStatus = queryDto.ConfirmationStatus,
                isNew = queryDto.IsNew,
                facultyId = queryDto.FacultyId > 0 ? queryDto.FacultyId : (int?)null
            }
        };
    }

    public async Task<object> UpdateChoiceAsync(ConfirmOrRejectChoiceDto[] items)
    {
        var results = new List<object>();
        var errors = new List<object>();

        foreach (var dto in items)
        {
            var bind = await _context.BindAddDisciplines
                .Include(b => b.Student)
                .Include(b => b.AddDisciplines)
                .FirstOrDefaultAsync(b => b.IdBindAddDisciplines == dto.BindId);

            if (bind == null)
            {
                errors.Add(new { bindId = dto.BindId, error = "Bind not found" });
                continue;
            }

            var action = dto.IsConfirm;
            if (action == 1)
            {
                bind.InProcess = 0;
                results.Add(new
                {
                    message = "Choice confirmed",
                    bindId = bind.IdBindAddDisciplines,
                    disciplineName = bind.AddDisciplines?.NameAddDisciplines
                });
                continue;
            }

            if (action == 0)
            {
                var student = bind.Student;
                var userId = student?.UserId ?? 0;
                var disciplineName = bind.AddDisciplines?.NameAddDisciplines ?? "elective";

                _context.BindAddDisciplines.Remove(bind);

                var notification = new Notification
                {
                    UserId = userId,
                    TemplateId = null,
                    CustomTitle = "Elective discipline rejected",
                    CustomMessage = $"Your choice \"{disciplineName}\" was rejected by the administrator.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    NotificationType = "DisciplineRejected",
                    Metadata = null
                };
                _context.Notifications.Add(notification);

                results.Add(new
                {
                    message = "Choice rejected and student notified",
                    bindId = dto.BindId,
                    disciplineName,
                    notificationId = notification.IdNotification
                });
                continue;
            }

            errors.Add(new { bindId = dto.BindId, error = "Action must be 0 (Reject) or 1 (Confirm)" });
        }

        await _context.SaveChangesAsync();

        return new { results, errors = errors.Count > 0 ? errors : null };
    }

    public async Task<object?> GetDisciplinesWithStatusAsync(GetDisciplinesWithStatusQueryDto queryDto)
    {
        var query = _context.AddDisciplines
            .Include(d => d.Faculty)
            .Include(d => d.DegreeLevel)
            .Include(d => d.Type)
            .Include(d => d.AddDetail)
                .ThenInclude(a => a!.Department)
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
                .Select(f => int.TryParse(f.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
            if (facultyIds.Any())
                query = query.Where(d => facultyIds.Contains(d.FacultyId));
        }

        if (queryDto.IsFaculty.HasValue)
            query = query.Where(d => d.IsFaculty == queryDto.IsFaculty.Value);

        if (!string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds))
        {
            var levelIds = queryDto.DegreeLevelIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => int.TryParse(d.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
            if (levelIds.Any())
                query = query.Where(d => d.DegreeLevelId.HasValue && levelIds.Contains(d.DegreeLevelId.Value));
        }

        var disciplines = await query.ToListAsync();

        var facultyIdsInList = disciplines.Select(d => d.FacultyId).Distinct().ToList();
        var lastPeriodByFaculty = await _context.DisciplineChoicePeriods
            .Where(p => p.FacultyId != null && facultyIdsInList.Contains(p.FacultyId.Value))
            .GroupBy(p => p.FacultyId!.Value)
            .Select(g => new { FacultyId = g.Key, StartDate = g.Max(p => p.StartDate) })
            .ToDictionaryAsync(x => x.FacultyId, x => x.StartDate);

        var normativeByLevelAndFaculty = await _context.Normatives
            .Where(n => n.DegreeLevelId != null)
            .Select(n => new { n.DegreeLevelId, n.IsFaculty, n.Count })
            .ToListAsync();
        var normativeLookup = normativeByLevelAndFaculty
            .GroupBy(n => (n.DegreeLevelId!.Value, n.IsFaculty))
            .ToDictionary(g => g.Key, g => g.First().Count);

        var disciplineIds = disciplines.Select(d => d.IdAddDisciplines).ToList();
        var facultyByDiscipline = disciplines.ToDictionary(d => d.IdAddDisciplines, d => d.FacultyId);
        var allBindsInScope = await _context.BindAddDisciplines
            .Where(b => disciplineIds.Contains(b.AddDisciplinesId))
            .Select(b => new { b.AddDisciplinesId, b.CreatedAt })
            .ToListAsync();

        var countByDiscipline = disciplineIds.ToDictionary(id => id, _ => 0);
        foreach (var b in allBindsInScope)
        {
            if (facultyByDiscipline.TryGetValue(b.AddDisciplinesId, out var facultyId)
                && lastPeriodByFaculty.TryGetValue(facultyId, out var periodStart)
                && b.CreatedAt >= periodStart)
            {
                countByDiscipline[b.AddDisciplinesId]++;
            }
        }

        var fullList = new List<AdminDisciplineListItemDto>();
        foreach (var d in disciplines)
        {
            var currentCount = countByDiscipline.TryGetValue(d.IdAddDisciplines, out var c) ? c : 0;
            var normativeCount = d.DegreeLevelId.HasValue
                && normativeLookup.TryGetValue((d.DegreeLevelId.Value, d.IsFaculty), out var norm)
                ? norm
                : (int?)null;

            string statusStr;
            if (d.IsForseChange == 1)
            {
                statusStr = d.Type?.TypeName ?? string.Empty;
            }
            else if (normativeCount == null || normativeCount == 0)
            {
                statusStr = currentCount >= (d.MinCountPeople ?? 0) ? "Accepted" : "Not Acquired";
            }
            else
            {
                var ratio = (double)currentCount / normativeCount;
                statusStr = ratio >= 1.0 ? "Accepted" : ratio >= 0.8 ? "Smartly Acquired" : "Not Acquired";
            }

            fullList.Add(new AdminDisciplineListItemDto
            {
                IdAddDisciplines = d.IdAddDisciplines,
                NameAddDisciplines = d.NameAddDisciplines,
                Teachers = d.AddDetail?.Teachers,
                DepartmentName = d.AddDetail?.Department?.NameDepartment,
                Credits = 5,
                Normative = normativeCount,
                MaxCountPeople = d.MaxCountPeople,
                CurrentCount = currentCount,
                Status = statusStr,
                IsForceChange = d.IsForseChange,
                DegreeLevelId = d.DegreeLevelId,
                IsFaculty = d.IsFaculty,
                FacultyId = d.FacultyId,
                FacultyAbbreviation = d.Faculty?.Abbreviation
            });
        }

        if (queryDto.StatusFilter.HasValue && queryDto.StatusFilter.Value >= 1 && queryDto.StatusFilter.Value <= 4)
        {
            var statusMap = new[] { "Empty", "Not Acquired", "Smartly Acquired", "Accepted", "Collected" };
            var filterStatus = statusMap[queryDto.StatusFilter.Value];
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
        var paginated = fullList
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        return new
        {
            totalPages,
            totalItems,
            currentPage = queryDto.Page,
            pageSize = queryDto.PageSize,
            disciplines = paginated,
            filters = new
            {
                faculties = string.IsNullOrWhiteSpace(queryDto.Faculties) ? null : queryDto.Faculties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList(),
                isFaculty = queryDto.IsFaculty,
                degreeLevelIds = string.IsNullOrWhiteSpace(queryDto.DegreeLevelIds) ? null : queryDto.DegreeLevelIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => int.Parse(d.Trim())).ToList(),
                search = string.IsNullOrWhiteSpace(queryDto.Search) ? null : queryDto.Search,
                statusFilter = queryDto.StatusFilter,
                sortOrder = queryDto.SortOrder
            }
        };
    }

    public async Task<object?> UpdateDisciplineStatusAsync(UpdateDisciplineStatusDto dto)
    {
        var discipline = await _context.AddDisciplines.FindAsync(dto.DisciplineId);
        if (discipline == null)
            return null;

        discipline.IsForseChange = 1;
        discipline.TypeId = dto.Status;
        await _context.SaveChangesAsync();

        var statusNames = new[] { "Empty", "Not Acquired", "Smartly Acquired", "Accepted", "Collected" };
        return new
        {
            message = "Discipline status updated",
            disciplineId = discipline.IdAddDisciplines,
            status = statusNames[dto.Status],
            isForceChange = 1
        };
    }

    public async Task<BindAddDisciplineDto?> GetBindAsync(int id)
    {
        var bind = await _context.BindAddDisciplines
            .Include(b => b.Student)
            .Include(b => b.AddDisciplines)
            .FirstOrDefaultAsync(b => b.IdBindAddDisciplines == id);

        if (bind == null)
            return null;

        return new BindAddDisciplineDto
        {
            IdBindAddDisciplines = bind.IdBindAddDisciplines,
            StudentId = bind.StudentId,
            StudentFullName = bind.Student?.NameStudent ?? "",
            AddDisciplinesId = bind.AddDisciplinesId,
            AddDisciplineName = bind.AddDisciplines?.NameAddDisciplines ?? "",
            Semestr = bind.Semestr,
            Loans = bind.Loans,
            InProcess = bind.InProcess == 1
        };
    }

    public async Task<StudentWithDisciplineChoicesDto?> GetStudentWithChoicesAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Faculty)
            .Include(s => s.Group)
            .Include(s => s.EducationalDegree)
            .Include(s => s.EducationalProgram)
            .Include(s => s.BindAddDisciplines)
                .ThenInclude(b => b.AddDisciplines)
            .FirstOrDefaultAsync(s => s.IdStudent == studentId);

        if (student == null)
            return null;

        var selected = student.BindAddDisciplines
            .Select(b => new StudentSelectedDisciplineDto
            {
                IdBindAddDisciplines = b.IdBindAddDisciplines,
                IdAddDisciplines = b.AddDisciplinesId,
                NameAddDisciplines = b.AddDisciplines?.NameAddDisciplines ?? "",
                CodeAddDisciplines = b.AddDisciplines?.CodeAddDisciplines ?? "",
                Semestr = b.Semestr,
                InProcess = b.InProcess
            })
            .ToList();

        var semestersWithChoices = selected.Select(x => x.Semestr).Distinct().ToList();
        var selectionOk = true;
        if (student.EducationalProgram != null)
        {
            foreach (var sem in semestersWithChoices)
            {
                var required = GetRequiredCountForSemester(student.EducationalProgram, sem);
                var count = selected.Count(d => d.Semestr == sem);
                if (count < required)
                {
                    selectionOk = false;
                    break;
                }
            }
        }

        var confirmationOk = selected.Count == 0 || selected.All(d => d.InProcess == 0);

        return new StudentWithDisciplineChoicesDto
        {
            StudentId = student.IdStudent,
            FullName = student.NameStudent ?? "",
            Faculty = student.Faculty?.Abbreviation ?? student.Faculty?.NameFaculty ?? "",
            Group = student.Group?.GroupCode ?? "",
            Year = student.Course,
            DegreeLevelId = student.EducationalDegreeId,
            DegreeLevelName = student.EducationalDegree?.NameEducationalDegreec ?? "",
            SelectedDisciplines = selected,
            SelectionStatus = selectionOk ? 1 : 0,
            ConfirmationStatus = confirmationOk ? 1 : 0
        };
    }

    public async Task<(int? bindId, string? error)> CreateBindAsync(AddDisciplineBindDto dto)
    {
        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null)
            return (null, "Student not found");

        var discipline = await _context.AddDisciplines.FindAsync(dto.DisciplineId);
        if (discipline == null)
            return (null, "Discipline not found");

        var exists = await _context.BindAddDisciplines
            .AnyAsync(b => b.StudentId == dto.StudentId && b.AddDisciplinesId == dto.DisciplineId);
        if (exists)
            return (null, "This student is already bound to this discipline");

        if (dto.Semestr < 1 || dto.Semestr > 8)
            return (null, "Semestr must be between 1 and 8");

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
        var bind = await _context.BindAddDisciplines
            .Include(b => b.Student)
            .Include(b => b.AddDisciplines)
            .FirstOrDefaultAsync(b => b.IdBindAddDisciplines == id);

        if (bind == null)
            return false;

        _context.BindAddDisciplines.Remove(bind);
        await _context.SaveChangesAsync();
        return true;
    }
}
