using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;
using OlimpBack.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

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

        [HttpGet("GetDisciplinesBySemester")]
        public async Task<ActionResult<DisciplineTabResponseDto>> GetDisciplinesBySemester(
            [FromQuery] int studentId,
            [FromQuery] bool isEvenSemester)
        {
            // Get the student with educational degree to determine their current course
            var student = await _context.Students
                .Include(s => s.EducationalDegree)
                .FirstOrDefaultAsync(s => s.IdStudents == studentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Calculate the current course based on education start date
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var yearsDifference = currentDate.Year - student.EducationStart.Year;
            var currentCourse = yearsDifference + 1;

            // Get all available disciplines that match the semester parity and degree level
            var disciplines = await _context.AddDisciplines
                .Where(d => d.AddSemestr == (isEvenSemester ? (sbyte)0 : (sbyte)1)) // Check semester type (0 for even, 1 for odd)
                .Where(d => d.DegreeLevel == student.EducationalDegree.NameEducationalDegreec) // Check degree level
                .ToListAsync();

            // Map the result to DTO
            var response = _mapper.Map<DisciplineTabResponseDto>((student, disciplines, currentCourse, isEvenSemester));

            return Ok(response);
        }
    }
} 