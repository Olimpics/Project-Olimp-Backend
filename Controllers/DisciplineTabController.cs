using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Linq.Expressions;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public DisciplineTabController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetAllDisciplinesWithAvailability")]
        public async Task<ActionResult<object>> GetAllDisciplinesWithAvailability(
            [FromQuery] int studentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] bool onlyAvailable = false,
            [FromQuery] string? search = null,
            [FromQuery] string? faculties = null,
            [FromQuery] string? courses = null,
            [FromQuery] bool? isEvenSemester = null,
            [FromQuery] string? degreeLevelIds = null,
            [FromQuery] int sortOrder = 0)
        {
            var context = await DisciplineAvailabilityService.BuildAvailabilityContext(studentId, _context);
            if (context == null)
                return NotFound("Student not found");

            var query = _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .Include(d => d.Faculty)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(d =>
                    EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
            }

            // Apply faculty filter
            if (!string.IsNullOrWhiteSpace(faculties))
            {
                var facultyIds = faculties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.Parse(id.Trim()))
                    .ToList();

                query = query.Where(d => facultyIds.Contains(d.FacultyId));
            }

            // Apply course filter
            if (!string.IsNullOrWhiteSpace(courses))
            {
                var courseList = courses.Split(',').Select(int.Parse).ToList();
                query = query.Where(d =>
                    (!d.MinCourse.HasValue || courseList.Contains(d.MinCourse.Value)) &&
                    (!d.MaxCourse.HasValue || courseList.Contains(d.MaxCourse.Value)));
            }

            // Apply semester filter
            if (isEvenSemester.HasValue)
            {
                var semesterValue = isEvenSemester.Value ? (sbyte)0 : (sbyte)1;
                query = query.Where(d => d.AddSemestr == semesterValue);
            }

            // Apply degree level filter
            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var levelIds = degreeLevelIds.Split(',').Select(int.Parse).ToList();

                var parameter = Expression.Parameter(typeof(AddDiscipline), "d");
                var property = Expression.Property(parameter, nameof(AddDiscipline.DegreeLevelId));

                // Привести levelId к int? для корректного сравнения
                var equalsExpressions = levelIds.Select(levelId =>
                    Expression.Equal(
                        property,
                        Expression.Constant((int?)levelId, typeof(int?))
                    )
                );

                // Объединить через OR
                Expression combinedOr = equalsExpressions.Aggregate((a, b) => Expression.OrElse(a, b));

                var lambda = Expression.Lambda<Func<AddDiscipline, bool>>(combinedOr, parameter);

                // Применить фильтр к запросу
                query = query.Where(lambda);
            }


            // Load filtered disciplines
            var allDisciplines = await query
                .ToListAsync();

            // Map and calculate availability
            var fullList = allDisciplines.Select(discipline =>
            {
                var dto = _mapper.Map<FullDisciplineDto>(discipline);
                dto.CountOfPeople = context.DisciplineCounts.TryGetValue(discipline.IdAddDisciplines, out var c) ? c : 0;
                dto.IsAvailable = DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context);
                return dto;
            }).ToList();

            // Filter by availability if needed
            if (onlyAvailable)
            {
                fullList = fullList.Where(d => d.IsAvailable).ToList();
            }

            fullList = sortOrder switch
            {
                1 => fullList.OrderByDescending(d => d.NameAddDisciplines).ToList(),
                2 => fullList.OrderBy(d => d.CountOfPeople).ToList(),
                3 => fullList.OrderByDescending(d => d.CountOfPeople).ToList(),
                4 => fullList.OrderBy(d => d.FacultyAbbreviation).ToList(),
                _ => fullList.OrderBy(d => d.NameAddDisciplines).ToList()
            };
            var totalItems = fullList.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = fullList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                totalPages,
                totalItems,
                currentPage = page,
                pageSize,
                disciplines = paginatedResult,
                filters = new
                {
                    faculties = faculties?.Split(',').Select(f => f.Trim()).ToList(),
                    courses = courses?.Split(',').Select(int.Parse).ToList(),
                    isEvenSemester,
                    degreeLevelIds = degreeLevelIds?.Split(',').Select(int.Parse).ToList()
                }
            };

            return Ok(response);
        }

       

        [HttpGet("GetDisciplinesBySemester")]
        public async Task<ActionResult<DisciplineTabResponseDto>> GetDisciplinesBySemester(
    [FromQuery] int studentId,
    [FromQuery] bool isEvenSemester)
        {
            var context = await DisciplineAvailabilityService.BuildAvailabilityContext(studentId, _context);
            if (context == null)
                return NotFound("Student not found");

            var disciplines = await _context.AddDisciplines
                .Where(d => d.AddSemestr == (isEvenSemester ? (sbyte)0 : (sbyte)1))
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

            return Ok(new DisciplineTabResponseDto
            {
                StudentId = context.Student.IdStudent,
                StudentName = context.Student.NameStudent,
                CurrentCourse = context.CurrentCourse,
                IsEvenSemester = isEvenSemester,
                Disciplines = availableDisciplines
            });
        }

        [HttpPost("AddDisciplineBind")]
        public async Task<ActionResult> AddDisciplineBind(AddDisciplineBindDto dto)
        {
            try
            {
                var context = await DisciplineAvailabilityService.BuildAvailabilityContext(dto.StudentId, _context);
                if (context == null)
                    return NotFound(new { error = $"Student not found {dto.StudentId}" });

                if (dto.Semester != 0 && dto.Semester != 1)
                    return BadRequest(new { error = "Semester must be 0 or 1" });

                int targetCourse = context.CurrentCourse + 1;
                int targetSemester = (targetCourse * 2) - dto.Semester;

                if (targetCourse > 4)
                    return BadRequest(new { error = "You can't choose disciplines in 5th course" });

                if (targetSemester > 8)
                    return BadRequest(new { error = $"Invalid semester: {targetSemester}" });

                var discipline = await _context.AddDisciplines
                    .FirstOrDefaultAsync(d => d.IdAddDisciplines == dto.DisciplineId);

                if (discipline == null)
                    return NotFound(new { error = "Discipline not found" });

                if (context.BoundDisciplineIds.Contains(dto.DisciplineId))
                    return BadRequest(new { error = "Student is already enrolled in this discipline" });

                if (!DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context))
                    return BadRequest(new { error = "Discipline is not available for this student" });

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

                return Ok(new
                {
                    message = "Discipline successfully bound to student",
                    bindId = bind.IdBindAddDisciplines
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while processing your request",
                    details = ex.Message
                });
            }
        }

        [HttpGet("GetDisciplineWithDetails/{id}")]
        public async Task<ActionResult<FullDisciplineWithDetailsDto>> GetDisciplineWithDetails(int id)
        {
            var discipline = await _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .Include(d => d.AddDetail)
                    .ThenInclude(d => d.Department)
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

            if (discipline == null)
                return NotFound("Discipline not found");

            if (discipline.AddDetail == null)
                return NotFound("Discipline details not found");

            var result = _mapper.Map<FullDisciplineWithDetailsDto>((discipline, discipline.AddDetail));
            return Ok(result);
        }

        [HttpPost("CreateDisciplineWithDetails")]
        public async Task<ActionResult<FullDisciplineWithDetailsDto>> CreateDisciplineWithDetails(CreateAddDisciplineWithDetailsDto dto)
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

                var result = _mapper.Map<FullDisciplineWithDetailsDto>((createdDiscipline, createdDiscipline.AddDetail));
                return CreatedAtAction(nameof(GetDisciplineWithDetails), new { id = discipline.IdAddDisciplines }, result);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPut("UpdateDisciplineWithDetails/{id}")]
        public async Task<IActionResult> UpdateDisciplineWithDetails(int id, UpdateAddDisciplineWithDetailsDto dto)
        {
            if (id != dto.IdAddDisciplines)
                return BadRequest();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var discipline = await _context.AddDisciplines
                    .Include(d => d.AddDetail)
                    .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

                if (discipline == null)
                    return NotFound("Discipline not found");

                if (discipline.AddDetail == null)
                    return NotFound("Discipline details not found");

                _mapper.Map(dto, discipline, opts => opts.Items["DbContext"] = _context);
                _mapper.Map(dto.Details, discipline.AddDetail);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpDelete("DeleteDisciplineWithDetails/{id}")]
        public async Task<IActionResult> DeleteDisciplineWithDetails(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var discipline = await _context.AddDisciplines
                    .Include(d => d.AddDetail)
                    .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

                if (discipline == null)
                    return NotFound("Discipline not found");

                if (discipline.AddDetail != null)
                {
                    _context.AddDetails.Remove(discipline.AddDetail);
                }

                _context.AddDisciplines.Remove(discipline);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}