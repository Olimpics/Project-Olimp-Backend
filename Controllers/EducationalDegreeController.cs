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
    public class EducationalDegreeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EducationalDegreeController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationalDegreeDto>>> GetEducationalDegrees()
        {
            var degrees = await _context.EducationalDegrees
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<EducationalDegreeDto>>(degrees));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EducationalDegreeDto>> GetEducationalDegree(int id)
        {
            var degree = await _context.EducationalDegrees
                .FirstOrDefaultAsync(ed => ed.IdEducationalDegree == id);

            if (degree == null)
                return NotFound();

            return Ok(_mapper.Map<EducationalDegreeDto>(degree));
        }

        [HttpPost]
        public async Task<ActionResult<EducationalDegreeDto>> CreateEducationalDegree(CreateEducationalDegreeDto dto)
        {
            var entity = _mapper.Map<EducationalDegree>(dto);
            _context.EducationalDegrees.Add(entity);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<EducationalDegreeDto>(entity);
            return CreatedAtAction(nameof(GetEducationalDegree), new { id = entity.IdEducationalDegree }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationalDegree(int id, UpdateEducationalDegreeDto dto)
        {
            if (id != dto.IdEducationalDegree)
                return BadRequest();

            var entity = await _context.EducationalDegrees.FindAsync(id);
            if (entity == null)
                return NotFound();

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationalDegree(int id)
        {
            var degree = await _context.EducationalDegrees.FindAsync(id);
            if (degree == null)
                return NotFound();

            _context.EducationalDegrees.Remove(degree);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EducationalDegreeExists(int id)
        {
            return _context.EducationalDegrees.Any(e => e.IdEducationalDegree == id);
        }
    }

}