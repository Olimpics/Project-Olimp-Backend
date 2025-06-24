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
using Microsoft.Data.SqlClient;

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
            [FromQuery] string? degreeLevelIds = null,
            [FromQuery] int sortOrder = 0)
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
                var facultyValues = faculties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList();
                var numericValues = facultyValues.Where(f => int.TryParse(f, out _)).Select(int.Parse).ToList();
                var textValues = facultyValues.Where(f => !int.TryParse(f, out _)).Select(f => f.ToLower()).ToList();

                if (numericValues.Any())
                {
                    query = query.Where(s => numericValues.Contains(s.FacultyId));
                }

                if (textValues.Any())
                {
                    query = query.Where(s =>
                        textValues.Any(t =>
                            EF.Functions.Like(s.Faculty.NameFaculty.ToLower(), $"%{t}%") ||
                            EF.Functions.Like(s.Faculty.Abbreviation.ToLower(), $"%{t}%")));
                }
            }

            // Apply speciality filter
            if (!string.IsNullOrWhiteSpace(speciality))
            {
                var specialityValues = speciality
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim().ToLower())
                    .ToList();

                if (specialityValues.Any())
                {
                    
                    var parameter = Expression.Parameter(typeof(Student), "s");
                    var property = Expression.Property(
                        Expression.Property(parameter, "EducationalProgram"),
                        nameof(EducationalProgram.SpecialityCode)
                    );

                    var toLowerCall = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

                    Expression? combinedExpression = null;
                    foreach (var val in specialityValues)
                    {
                        var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
                        var startsWithCall = Expression.Call(toLowerCall, startsWithMethod, Expression.Constant(val));

                        combinedExpression = combinedExpression == null
                            ? startsWithCall
                            : Expression.OrElse(combinedExpression, startsWithCall);
                    }

                    var lambda = Expression.Lambda<Func<Student, bool>>(combinedExpression!, parameter);
                    query = query.Where(lambda);
                }
            }



            // Apply group filter
            if (!string.IsNullOrWhiteSpace(group))
            {
                var groupIdList = group
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => int.TryParse(g.Trim(), out var id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (groupIdList.Any())
                {
                    query = query.Where(s => groupIdList.Contains(s.Group.IdGroup));
                }
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

            // Apply pagination
            var students = await query.ToListAsync();
            students = sortOrder switch
            {
                1 => students.OrderByDescending(d => d.NameStudent).ToList(),
                2 => students.OrderBy(d => d.Faculty.Abbreviation).ToList(),
                3 => students.OrderByDescending(d => d.Faculty.Abbreviation).ToList(),
                4 => students.OrderBy(d => d.Group.GroupCode).ToList(),
                5 => students.OrderByDescending(d => d.Group.GroupCode).ToList(),
                _ => students.OrderBy(d => d.NameStudent).ToList()
            };
            // Get total count for pagination
            var totalItems = students.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = students
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToList();

            var studentDtos = _mapper.Map<IEnumerable<StudentForCatalogDto>>(paginatedResult);

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
                .Where(s => s.IdStudent == id)
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
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] List<CreateStudentDto> dtos)
        {
            var results = new List<StudentDto>();
            var student = new Student();
            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.NameStudent))
                    continue;
                student = await _context.Students
                    .FirstOrDefaultAsync(s => s.NameStudent == dto.NameStudent && s.IdStudent == dto.IdStudent);
                    int userId = dto.UserId;
                    if (userId == 0)
                        userId = await UserService.CreateUserForStudent(dto.NameStudent, _context);
                    dto.UserId = userId;
                    student = _mapper.Map<Student>(dto);
                    _context.Students.Add(student);
            }

            await _context.SaveChangesAsync();
            return Ok(results);
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
            return _context.Students.Any(e => e.IdStudent == id);
        }
    }
}
