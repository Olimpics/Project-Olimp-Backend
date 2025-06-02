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
            [FromQuery] string? degreeLevelIds = null)
        {
            var context = await DisciplineAvailabilityService.BuildAvailabilityContext(studentId, _context);
            if (context == null)
                return NotFound("Student not found");

            var query = _context.AddDisciplines
                .Include(d => d.DegreeLevel)
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
                var facultyList = faculties.Split(',').Select(f => f.Trim()).ToList();
                Expression<Func<AddDiscipline, bool>> facultyPredicate = d => false;
                var parameter = Expression.Parameter(typeof(AddDiscipline), "d");
                
                var conditions = facultyList.Select(faculty =>
                    Expression.Equal(
                        Expression.Property(parameter, "Faculty"),
                        Expression.Constant(faculty)
                    )
                );
                
                var orExpression = conditions.Aggregate((a, b) => Expression.OrElse(a, b));
                facultyPredicate = Expression.Lambda<Func<AddDiscipline, bool>>(orExpression, parameter);
                
                query = query.Where(facultyPredicate);
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

                // Приводим levelId к типу property.Type (вдруг это int? или что-то ещё)
                var conditions = levelIds.Select(levelId =>
                    Expression.Equal(
                        property,
                        Expression.Constant(Convert.ChangeType(levelId, property.Type), property.Type)
                    )
                );

                var orExpression = conditions.Aggregate((a, b) => Expression.OrElse(a, b));
                var levelPredicate = Expression.Lambda<Func<AddDiscipline, bool>>(orExpression, parameter);

                query = query.Where(levelPredicate);
            }


            // Load filtered disciplines
            var allDisciplines = await query
                .OrderBy(d => d.NameAddDisciplines)
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
                StudentId = context.Student.IdStudents,
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
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

            if (discipline == null)
                return NotFound("Discipline not found");

            var details = await _context.AddDetails
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.IdAddDetails == id);

            if (details == null)
                return NotFound("Discipline details not found");

            var result = _mapper.Map<FullDisciplineWithDetailsDto>((discipline, details));
            return Ok(result);
        }
    }
}