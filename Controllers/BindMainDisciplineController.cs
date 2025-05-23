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
    public class BindMainDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BindMainDisciplineController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BindMainDisciplineDto>>> GetBindMainDisciplines()
        {
            var entities = await _context.BindMainDisciplines
                .Include(bmd => bmd.EducationalProgram)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<BindMainDisciplineDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BindMainDisciplineDto>> GetBindMainDiscipline(int id)
        {
            var entity = await _context.BindMainDisciplines
                .Include(bmd => bmd.EducationalProgram)
                .FirstOrDefaultAsync(bmd => bmd.IdBindMainDisciplines == id);

            if (entity == null)
                return NotFound();

            return Ok(_mapper.Map<BindMainDisciplineDto>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<BindMainDisciplineDto>> CreateBindMainDiscipline(CreateBindMainDisciplineDto dto)
        {
            var entity = _mapper.Map<BindMainDiscipline>(dto);
            _context.BindMainDisciplines.Add(entity);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<BindMainDisciplineDto>(entity);
            return CreatedAtAction(nameof(GetBindMainDiscipline), new { id = entity.IdBindMainDisciplines }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindMainDiscipline(int id, UpdateBindMainDisciplineDto dto)
        {
            if (id != dto.IdBindMainDisciplines)
                return BadRequest();

            var entity = await _context.BindMainDisciplines.FindAsync(id);
            if (entity == null)
                return NotFound();

            _mapper.Map(dto, entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!BindMainDisciplineExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBindMainDiscipline(int id)
        {
            var entity = await _context.BindMainDisciplines.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.BindMainDisciplines.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BindMainDisciplineExists(int id)
        {
            return _context.BindMainDisciplines.Any(e => e.IdBindMainDisciplines == id);
        }
    }

}