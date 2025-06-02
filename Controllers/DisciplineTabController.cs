using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
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
    [FromQuery] string? search = null)
        {
            var context = await DisciplineAvailabilityService.BuildAvailabilityContext(studentId, _context);
            if (context == null)
                return NotFound("Student not found");

            var query = _context.AddDisciplines
                .Include(d => d.DegreeLevelId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(d =>
                    d.NameAddDisciplines.ToLower().Contains(lowerSearch) ||
                    d.CodeAddDisciplines.ToLower().Contains(lowerSearch));
            }

            // Загружаем все подходящие дисциплины
            var allDisciplines = await query
                .OrderBy(d => d.NameAddDisciplines)
                .ToListAsync();

            // Маппим и вычисляем доступность
            var fullList = allDisciplines.Select(discipline =>
            {
                var dto = _mapper.Map<FullDisciplineDto>(discipline);
                dto.CountOfPeople = context.DisciplineCounts.TryGetValue(discipline.IdAddDisciplines, out var c) ? c : 0;
                dto.IsAvailable = DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context);
                return dto;
            }).ToList();

            // Фильтруем, если нужно
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
                disciplines = paginatedResult
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