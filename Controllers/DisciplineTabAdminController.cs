using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using System.Linq.Expressions;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabAdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DisciplineTabAdminController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns required number of elective disciplines for a semester from the educational program.
        /// </summary>
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

        /// <summary>
        /// Get all students with their selected disciplines, selection and confirmation status, with filters and pagination.
        /// </summary>
        [HttpGet("GetStudentsWithDisciplineChoices")]
        public async Task<ActionResult<object>> GetStudentsWithDisciplineChoices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            [FromQuery] string? search = null,
            [FromQuery] string? faculties = null,
            [FromQuery] string? courses = null,
            [FromQuery] string? groups = null,
            [FromQuery] string? degreeLevelIds = null,
            [FromQuery] int? selectionStatus = null,
            [FromQuery] int? confirmationStatus = null,
            [FromQuery] int sortOrder = 0,
            [FromQuery] int isNew = 1,
            [FromQuery] int facultyId = 0)
        {
            var query = _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .Include(s => s.EducationalDegree)
                .Include(s => s.EducationalProgram)
                .Include(s => s.BindAddDisciplines)
                    .ThenInclude(b => b.AddDisciplines)
                .AsQueryable();

            // Search by full name, faculty name/abbreviation, group code
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(s =>
                    EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%") ||
                    (s.Faculty != null && (
                        EF.Functions.Like(s.Faculty.NameFaculty.ToLower(), $"%{lowerSearch}%") ||
                        EF.Functions.Like(s.Faculty.Abbreviation.ToLower(), $"%{lowerSearch}%"))) ||
                    (s.Group != null && EF.Functions.Like(s.Group.GroupCode.ToLower(), $"%{lowerSearch}%")));
            }

            // Filter by faculty ids
            if (!string.IsNullOrWhiteSpace(faculties))
            {
                var facultyIds = faculties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => int.TryParse(f.Trim(), out var id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (facultyIds.Any())
                    query = query.Where(s => facultyIds.Contains(s.FacultyId));
            }

            // Filter by course (year)
            if (!string.IsNullOrWhiteSpace(courses))
            {
                var courseList = courses
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => int.TryParse(c.Trim(), out var id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (courseList.Any())
                    query = query.Where(s => courseList.Contains(s.Course));
            }

            // Filter by group ids
            if (!string.IsNullOrWhiteSpace(groups))
            {
                var groupIds = groups
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => int.TryParse(g.Trim(), out var id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (groupIds.Any())
                    query = query.Where(s => groupIds.Contains(s.GroupId));
            }

            // Filter by degree level ids
            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var levelIds = degreeLevelIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => int.TryParse(d.Trim(), out var id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (levelIds.Any())
                    query = query.Where(s => levelIds.Contains(s.EducationalDegreeId));
            }

            // When isNew=1, restrict to students of the given faculty and get last period start for filtering binds
            DateTime? periodStart = null;
            if (isNew == 1 && facultyId > 0)
            {
                query = query.Where(s => s.FacultyId == facultyId);
                var lastPeriod = await _context.DisciplineChoicePeriods
                    .Where(p => p.FacultyId == facultyId)
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
                if (isNew == 1 && periodStart.HasValue)
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

                var dto = new StudentWithDisciplineChoicesDto
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
                };
                items.Add(dto);
            }

            // Filter by selection status (0 or 1)
            if (selectionStatus.HasValue && (selectionStatus.Value == 0 || selectionStatus.Value == 1))
                items = items.Where(x => x.SelectionStatus == selectionStatus.Value).ToList();

            // Filter by confirmation status (0 or 1)
            if (confirmationStatus.HasValue && (confirmationStatus.Value == 0 || confirmationStatus.Value == 1))
                items = items.Where(x => x.ConfirmationStatus == confirmationStatus.Value).ToList();

            // Sort
            items = sortOrder switch
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
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var paginated = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                totalPages,
                totalItems,
                currentPage = page,
                pageSize,
                students = paginated,
                filters = new
                {
                    faculties = string.IsNullOrWhiteSpace(faculties) ? null : faculties.Split(',').Select(f => f.Trim()).ToList(),
                    courses = string.IsNullOrWhiteSpace(courses) ? null : courses.Split(',').Select(c => c.Trim()).ToList(),
                    groups = string.IsNullOrWhiteSpace(groups) ? null : groups.Split(',').Select(g => g.Trim()).ToList(),
                    degreeLevelIds = string.IsNullOrWhiteSpace(degreeLevelIds) ? null : degreeLevelIds.Split(',').Select(d => int.Parse(d.Trim())).ToList(),
                    selectionStatus,
                    confirmationStatus,
                    isNew,
                    facultyId = facultyId > 0 ? facultyId : (int?)null
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Confirm or reject student elective choices. Accepts an array of items (bindId, isConfirm).
        /// Confirm: set InProcess to 0. Reject: delete bind and notify student.
        /// </summary>
        [HttpPut("UpdateChoice")]
        public async Task<ActionResult<object>> UpdateChoice(ConfirmOrRejectChoiceDto[] items)
        {
            if (items == null || items.Length == 0)
                return BadRequest(new { error = "At least one item is required" });

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

            var response = new
            {
                results,
                errors = errors.Count > 0 ? errors : null
            };
            return Ok(response);
        }

        /// <summary>
        /// Returns disciplines with name, teachers, department, credits, min/max/current count, and status
        /// (Accepted = 100%+ of normative, Smartly Acquired = 80%+, Not Acquired = under 80%).
        /// Current count = binds created since the last opened period for the discipline's faculty.
        /// </summary>
        [HttpGet("GetDisciplinesWithStatus")]
        public async Task<ActionResult<object>> GetDisciplinesWithStatus(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            [FromQuery] string? search = null,
            [FromQuery] string? faculties = null,
            [FromQuery] sbyte? isFaculty = null,
            [FromQuery] string? degreeLevelIds = null,
            [FromQuery] int? statusFilter = null,
            [FromQuery] int sortOrder = 0)
        {
            var query = _context.AddDisciplines
                .Include(d => d.Faculty)
                .Include(d => d.DegreeLevel)
                .Include(d => d.AddDetail)
                    .ThenInclude(a => a!.Department)
                .AsQueryable();

            // Apply search filter (by name or code)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(d =>
                    EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
            }

            if (!string.IsNullOrWhiteSpace(faculties))
            {
                var facultyIds = faculties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => int.TryParse(f.Trim(), out var id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (facultyIds.Any())
                    query = query.Where(d => facultyIds.Contains(d.FacultyId));
            }

            if (isFaculty.HasValue)
                query = query.Where(d => d.IsFaculty == isFaculty.Value);

            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var levelIds = degreeLevelIds
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
                if (normativeCount == null || normativeCount == 0)
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

            if (statusFilter.HasValue && statusFilter.Value >= 1 && statusFilter.Value <= 4)
            {
                var statusMap = new[] {"Empty", "Not Acquired", "Smartly Acquired", "Accepted", "Collected"};
                var filterStatus = statusMap[statusFilter.Value];
                fullList = fullList.Where(x => x.Status == filterStatus).ToList();
            }

            fullList = sortOrder switch
            {
                1 => fullList.OrderByDescending(d => d.NameAddDisciplines).ToList(),
                2 => fullList.OrderBy(d => d.CurrentCount).ToList(),
                3 => fullList.OrderByDescending(d => d.CurrentCount).ToList(),
                _ => fullList.OrderBy(d => d.NameAddDisciplines).ToList()
            };

            var totalItems = fullList.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var paginated = fullList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                totalPages,
                totalItems,
                currentPage = page,
                pageSize,
                disciplines = paginated,
                filters = new
                {
                    faculties = string.IsNullOrWhiteSpace(faculties) ? null : faculties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList(),
                    isFaculty,
                    degreeLevelIds = string.IsNullOrWhiteSpace(degreeLevelIds) ? null : degreeLevelIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => int.Parse(d.Trim())).ToList(),
                    search = string.IsNullOrWhiteSpace(search) ? null : search,
                    statusFilter,
                    sortOrder
                }
            });
        }

        /// <summary>
        /// Updates discipline status (Selected, Intellectually Selected, Not Selected). Sets IsForceChange to 1 for the discipline.
        /// </summary>
        [HttpPut("UpdateDisciplineStatus")]
        public async Task<ActionResult<object>> UpdateDisciplineStatus(UpdateDisciplineStatusDto dto)
        {
            if (dto.Status < 1 || dto.Status > 4)
                return BadRequest(new { error = "Status must be 1 (Not Selected), 2 (Intellectually Selected), 3 (Selected) or 4 (Collected)" });

            var discipline = await _context.AddDisciplines.FindAsync(dto.DisciplineId);
            if (discipline == null)
                return NotFound(new { error = "Discipline not found" });

            discipline.IsForseChange = 1;
            discipline.TypeId = dto.Status;
            await _context.SaveChangesAsync();

            var statusNames = new[] { "Empty", "Not Acquired", "Smartly Acquired", "Accepted", "Collected" };
            return Ok(new
            {
                message = "Discipline status updated",
                disciplineId = discipline.IdAddDisciplines,
                status = statusNames[dto.Status],
                isForceChange = 1
            });
        }

        // --- Standard CRUD for bind (admin) ---

        [HttpGet("{id}")]
        public async Task<ActionResult<BindAddDisciplineDto>> GetBind(int id)
        {
            var bind = await _context.BindAddDisciplines
                .Include(b => b.Student)
                .Include(b => b.AddDisciplines)
                .FirstOrDefaultAsync(b => b.IdBindAddDisciplines == id);

            if (bind == null)
                return NotFound();

            var dto = new BindAddDisciplineDto
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
            return Ok(dto);
        }

        [HttpGet("GetStudentWithChoices/{studentId}")]
        public async Task<ActionResult<StudentWithDisciplineChoicesDto>> GetStudentWithChoices(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .Include(s => s.EducationalProgram)
                .Include(s => s.BindAddDisciplines)
                    .ThenInclude(b => b.AddDisciplines)
                .FirstOrDefaultAsync(s => s.IdStudent == studentId);

            if (student == null)
                return NotFound();

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

            var dto = new StudentWithDisciplineChoicesDto
            {
                StudentId = student.IdStudent,
                FullName = student.NameStudent ?? "",
                Faculty = student.Faculty?.Abbreviation ?? student.Faculty?.NameFaculty ?? "",
                Group = student.Group?.GroupCode ?? "",
                Year = student.Course,
                SelectedDisciplines = selected,
                SelectionStatus = selectionOk ? 1 : 0,
                ConfirmationStatus = confirmationOk ? 1 : 0
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateBind(AdminCreateBindDto dto)
        {
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
                return NotFound(new { error = "Student not found" });

            var discipline = await _context.AddDisciplines.FindAsync(dto.DisciplineId);
            if (discipline == null)
                return NotFound(new { error = "Discipline not found" });

            var exists = await _context.BindAddDisciplines
                .AnyAsync(b => b.StudentId == dto.StudentId && b.AddDisciplinesId == dto.DisciplineId);
            if (exists)
                return BadRequest(new { error = "This student is already bound to this discipline" });

            if (dto.Semestr < 1 || dto.Semestr > 8)
                return BadRequest(new { error = "Semestr must be between 1 and 8" });

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

            return CreatedAtAction(nameof(GetBind), new { id = bind.IdBindAddDisciplines }, new
            {
                message = "Bind created",
                bindId = bind.IdBindAddDisciplines
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBind(int id)
        {
            var bind = await _context.BindAddDisciplines
                .Include(b => b.Student)
                .Include(b => b.AddDisciplines)
                .FirstOrDefaultAsync(b => b.IdBindAddDisciplines == id);

            if (bind == null)
                return NotFound();

            _context.BindAddDisciplines.Remove(bind);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
