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
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Status)
                .Include(s => s.Faculty)
                .Include(s => s.EducationalProgram)
                .Include(s => s.EducationalDegree)
                .Include(s => s.StudyForm)
                .ToListAsync();

            // Calculate course for students with course = 0
            foreach (var student in students.Where(s => s.Course == 0))
            {
                student.Course = await CourseCalculator.CalculateCurrentCourse(student, _context);
            }
            await _context.SaveChangesAsync();

            var dtos = _mapper.Map<IEnumerable<StudentDto>>(students);
            return Ok(dtos);
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
