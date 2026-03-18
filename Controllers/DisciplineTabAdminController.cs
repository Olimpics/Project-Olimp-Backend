using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabAdminController : ControllerBase
    {
        private readonly IDisciplineTabAdminService _service;

        public DisciplineTabAdminController(IDisciplineTabAdminService service)
        {
            _service = service;
        }

        [HttpGet("GetAllDisciplines")]
        public async Task<ActionResult<object>> GetAllDisciplines(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 50,
           [FromQuery] string? search = null,
           [FromQuery] string? faculties = null,
           [FromQuery] string? courses = null,
           [FromQuery] bool? isEvenSemester = null,
           [FromQuery] string? degreeLevelIds = null,
           [FromQuery] int sortOrder = 0)
        {
            var query = _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .Include(d => d.Faculty)
                .Include(d => d.BindAddDisciplines)
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
                query = query.Where(d => d.AddSemestr.HasValue &&
                    ((isEvenSemester.Value && d.AddSemestr.Value % 2 == 0) ||
                     (!isEvenSemester.Value && d.AddSemestr.Value % 2 == 1)));
            }

            // Apply degree level filter
            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var degreeLevelIdList = degreeLevelIds.Split(',').Select(int.Parse).ToList();
                query = query.Where(d => d.DegreeLevelId.HasValue && degreeLevelIdList.Contains(d.DegreeLevelId.Value));
            }

            // Apply sorting
            query = sortOrder switch
            {
                1 => query.OrderBy(d => d.NameAddDisciplines),
                2 => query.OrderByDescending(d => d.NameAddDisciplines),
                3 => query.OrderBy(d => d.CodeAddDisciplines),
                4 => query.OrderByDescending(d => d.CodeAddDisciplines),
                _ => query.OrderBy(d => d.IdAddDisciplines)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var disciplines = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = disciplines.Select(d => _mapper.Map<FullDisciplineDto>(d)).ToList();

            // Add count of people for each discipline
            foreach (var discipline in result)
            {
                discipline.CountOfPeople = disciplines
                    .First(d => d.IdAddDisciplines == discipline.IdAddDisciplines)
                    .BindAddDisciplines.Count;
            }

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Items = result
            });
        }

        [HttpGet("GetStudentsWithDisciplineChoices")]
        public async Task<ActionResult<object>> GetStudentsWithDisciplineChoices(
            [FromQuery] GetStudentsWithDisciplineChoicesQueryDto query)
        {
            var result = await _service.GetStudentsWithDisciplineChoicesAsync(query);
            return Ok(result);
        }

        [HttpPut("UpdateChoice")]
        public async Task<ActionResult<object>> UpdateChoice(ConfirmOrRejectChoiceDto[] items)
        {
            if (items == null || items.Length == 0)
                return BadRequest(new { error = "At least one item is required" });

            var result = await _service.UpdateChoiceAsync(items);
            return Ok(result);
        }

        [HttpGet("GetDisciplinesWithStatus")]
        public async Task<ActionResult<object>> GetDisciplinesWithStatus(
            [FromQuery] GetDisciplinesWithStatusQueryDto query)
        {
            var result = await _service.GetDisciplinesWithStatusAsync(query);
            return Ok(result);
        }

        [HttpPut("UpdateDisciplineStatus")]
        public async Task<ActionResult<object>> UpdateDisciplineStatus(UpdateDisciplineStatusDto dto)
        {
            if (dto.Status < 1 || dto.Status > 4)
                return BadRequest(new { error = "Status must be 1 (Not Selected), 2 (Intellectually Selected), 3 (Selected) or 4 (Collected)" });

            var result = await _service.UpdateDisciplineStatusAsync(dto);
            if (result == null)
                return NotFound(new { error = "Discipline not found" });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BindAddDisciplineDto>> GetBind(int id)
        {
            var result = await _service.GetBindAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("GetStudentWithChoices/{studentId}")]
        public async Task<ActionResult<StudentWithDisciplineChoicesDto>> GetStudentWithChoices(int studentId)
        {
            var result = await _service.GetStudentWithChoicesAsync(studentId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateBindAddDiscipline(int id, UpdateBindAddDisciplineDto updateDto)
        //{
        //    var bind = await _context.BindAddDisciplines.FindAsync(id);
        //    if (bind == null)
        //    {
        //        return NotFound();
        //    }

        //    _mapper.Map(updateDto, bind);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
               
                
        //            throw;
                
        //    }

        //    return NoContent();
        //}


    }
}
