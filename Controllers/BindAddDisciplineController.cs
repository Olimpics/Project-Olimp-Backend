using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindAddDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BindAddDisciplineController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/BindAddDiscipline
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BindAddDisciplineDto>>> GetBindAddDisciplines()
        {
            var bindDisciplines = await _context.BindAddDisciplines
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<BindAddDisciplineDto>>(bindDisciplines);
            return Ok(dtos);
        }

        // GET: api/BindAddDiscipline/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BindAddDisciplineDto>> GetBindAddDiscipline(int id)
        {
            var bindDiscipline = await _context.BindAddDisciplines
                .Where(bad => bad.IdBindAddDisciplines == id)
                .Include(bad => bad.Student)
                .ProjectTo<BindAddDisciplineDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (bindDiscipline == null)
            {
                return NotFound();
            }

            return Ok(bindDiscipline);
        }

        // POST: api/BindAddDiscipline
        [HttpPost]
        public async Task<ActionResult<BindAddDisciplineDto>> CreateBindAddDiscipline(CreateBindAddDisciplineDto createDto)
        {
            var bind = _mapper.Map<BindAddDiscipline>(createDto);
            _context.BindAddDisciplines.Add(bind);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BindAddDisciplineDto>(bind);
            return CreatedAtAction(nameof(GetBindAddDiscipline), new { id = bind.IdBindAddDisciplines }, dto);
        }

        // PUT: api/BindAddDiscipline/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindAddDiscipline(int id, UpdateBindAddDisciplineDto updateDto)
        {
            var bind = await _context.BindAddDisciplines.FindAsync(id);
            if (bind == null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, bind);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BindAddDisciplineExists(id))
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

        // DELETE: api/BindAddDiscipline/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBindAddDiscipline(int id)
        {
            var bind = await _context.BindAddDisciplines.FindAsync(id);
            if (bind == null)
            {
                return NotFound();
            }

            _context.BindAddDisciplines.Remove(bind);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BindAddDisciplineExists(int id)
        {
            return _context.BindAddDisciplines.Any(e => e.IdBindAddDisciplines == id);
        }
    }
}
