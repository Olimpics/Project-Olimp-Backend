using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultyController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FacultyController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacultyDto>>> GetFaculties()
        {
            var faculties = await _context.Faculties.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<FacultyDto>>(faculties));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacultyDto>> GetFaculty(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
                return NotFound();

            return Ok(_mapper.Map<FacultyDto>(faculty));
        }

        [HttpPost]
        public async Task<ActionResult<FacultyDto>> CreateFaculty(FacultyDto facultyDto)
        {
            var faculty = _mapper.Map<Faculty>(facultyDto);
            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<FacultyDto>(faculty);
            return CreatedAtAction(nameof(GetFaculty), new { id = faculty.IdFaculty }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaculty(int id, FacultyDto facultyDto)
        {
            if (id != facultyDto.IdFaculty)
                return BadRequest();

            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
                return NotFound();

            _mapper.Map(facultyDto, faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaculty(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
                return NotFound();

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacultyExists(int id)
        {
            return _context.Faculties.Any(e => e.IdFaculty == id);
        }
    }

}