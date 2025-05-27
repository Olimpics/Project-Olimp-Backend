using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AddDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddDisciplineController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddDisciplineDto>>> GetAddDisciplines()
        {
            var disciplines = await _context.AddDisciplines
                .ProjectTo<AddDisciplineDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(disciplines);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AddDisciplineDto>> GetAddDiscipline(int id)
        {
            var discipline = await _context.AddDisciplines
                .Where(d => d.IdAddDisciplines == id)
                .ProjectTo<AddDisciplineDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (discipline == null)
            {
                return NotFound();
            }

            return Ok(discipline);
        }

        [HttpPost]
        public async Task<ActionResult<AddDisciplineDto>> CreateAddDiscipline(CreateAddDisciplineDto dto)
        {
            var discipline = _mapper.Map<AddDiscipline>(dto);
            _context.AddDisciplines.Add(discipline);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<AddDisciplineDto>(discipline);
            return CreatedAtAction(nameof(GetAddDiscipline), new { id = discipline.IdAddDisciplines }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddDiscipline(int id, CreateAddDisciplineDto dto)
        {
            var discipline = await _context.AddDisciplines.FindAsync(id);
            if (discipline == null) return NotFound();

            _mapper.Map(dto, discipline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddDiscipline(int id)
        {
            var discipline = await _context.AddDisciplines.FindAsync(id);
            if (discipline == null) return NotFound();

            _context.AddDisciplines.Remove(discipline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddDisciplineExists(int id)
        {
            return _context.AddDisciplines.Any(e => e.IdAddDisciplines == id);
        }

    } 
}