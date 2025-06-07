using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Student
        [HttpGet]
        public async Task<ActionResult<object>> GetStudents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] string? faculties = null,
            [FromQuery] string? speciality = null,
            [FromQuery] string? group = null,
            [FromQuery] string? courses = null,
            [FromQuery] string? studyForm = null,
            [FromQuery] string? degreeLevelIds = null)
        {
            var query = _context.Students
                .Include(s => s.Status)
                .Include(s => s.Faculty)
                .Include(s => s.EducationalProgram)
                .Include(s => s.EducationalDegree)
                .Include(s => s.Group)
                .Include(s => s.StudyForm)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(s =>
                    EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%"));
            }

            // Apply faculty filter
            if (!string.IsNullOrWhiteSpace(faculties))
            {
                var facultyValues = faculties.Split(',').Select(f => f.Trim()).ToList();
                var numericValues = facultyValues.Where(f => int.TryParse(f, out _)).Select(int.Parse).ToList();
                var textValues = facultyValues.Where(f => !int.TryParse(f, out _)).Select(f => f.ToLower()).ToList();

                if (numericValues.Any())
                {
                    query = query.Where(s => numericValues.Contains(s.FacultyId));
                }

                if (textValues.Any())
                {
                    foreach (var val in textValues)
                    {
                        var temp = val; // Avoid closure issue
                        query = query.Where(s =>
                            EF.Functions.Like(s.Faculty.NameFaculty.ToLower(), $"%{temp}%") ||
                            EF.Functions.Like(s.Faculty.Abbreviation.ToLower(), $"%{temp}%"));
                    }
                }
            }

            // Apply speciality filter
            if (!string.IsNullOrWhiteSpace(speciality))
            {
                query = query.Where(s => 
                    EF.Functions.Like(s.EducationalProgram.Speciality.ToLower(), $"%{speciality.ToLower()}%"));
            }

            // Apply group filter
            if (!string.IsNullOrWhiteSpace(group))
            {
                query = query.Where(s => 
                    EF.Functions.Like(s.GroupId, $"%{group.ToLower()}%"));
            }

            // Apply course filter
            if (!string.IsNullOrWhiteSpace(courses))
            {
                var courseList = courses.Split(',').Select(int.Parse).ToList();
                query = query.Where(s => courseList.Contains(s.Course));
            }

            // Apply study form filter
            if (!string.IsNullOrWhiteSpace(studyForm))
            {
                var studyFormIds = studyForm.Split(',').Select(int.Parse).ToList();
                query = query.Where(s => studyFormIds.Contains(s.StudyFormId));
            }

            // Apply degree level filter
            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var levelIds = degreeLevelIds.Split(',').Select(int.Parse).ToList();
                query = query.Where(s => levelIds.Contains(s.EducationalDegreeId));
            }

            // Get total count for pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Apply pagination
            var students = await query
                .OrderBy(s => s.NameStudent)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var studentDtos = _mapper.Map<IEnumerable<StudentForCatalogDto>>(students);

            var response = new
            {
                totalPages,
                totalItems,
                currentPage = page,
                pageSize,
                students = studentDtos,
                filters = new
                {
                    faculties = faculties?.Split(',').Select(f => f.Trim()).ToList(),
                    speciality,
                    group,
                    courses = courses?.Split(',').Select(int.Parse).ToList(),
                    studyForm = studyForm?.Split(',').Select(int.Parse).ToList(),
                    degreeLevelIds = degreeLevelIds?.Split(',').Select(int.Parse).ToList()
                }
            };

            return Ok(response);
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _context.Students
                .Where(s => s.IdStudents == id)
                .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST: api/Student
        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentDto createDto)
        {
            var student = _mapper.Map<Student>(createDto);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<StudentDto>(student);
            return CreatedAtAction(nameof(GetStudent), new { id = student.IdStudents }, dto);
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, UpdateStudentDto updateDto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, student);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.IdStudents == id);
        }
    }
}
