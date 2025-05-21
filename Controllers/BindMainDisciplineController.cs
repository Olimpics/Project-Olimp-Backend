using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindMainDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BindMainDisciplineController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BindMainDiscipline
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BindMainDiscipline>>> GetBindMainDisciplines()
        {
            return await _context.BindMainDisciplines
                .Include(bmd => bmd.EducationalProgram)
                .ToListAsync();
        }

        // GET: api/BindMainDiscipline/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BindMainDiscipline>> GetBindMainDiscipline(int id)
        {
            var bindDiscipline = await _context.BindMainDisciplines
                .Include(bmd => bmd.EducationalProgram)
                .FirstOrDefaultAsync(bmd => bmd.IdBindMainDisciplines == id);

            if (bindDiscipline == null)
            {
                return NotFound();
            }

            return bindDiscipline;
        }

        // POST: api/BindMainDiscipline
        [HttpPost]
        public async Task<ActionResult<BindMainDiscipline>> CreateBindMainDiscipline(BindMainDiscipline bindDiscipline)
        {
            _context.BindMainDisciplines.Add(bindDiscipline);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBindMainDiscipline), new { id = bindDiscipline.IdBindMainDisciplines }, bindDiscipline);
        }

        // PUT: api/BindMainDiscipline/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindMainDiscipline(int id, BindMainDiscipline bindDiscipline)
        {
            if (id != bindDiscipline.IdBindMainDisciplines)
            {
                return BadRequest();
            }

            _context.Entry(bindDiscipline).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BindMainDisciplineExists(id))
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

        // DELETE: api/BindMainDiscipline/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBindMainDiscipline(int id)
        {
            var bindDiscipline = await _context.BindMainDisciplines.FindAsync(id);
            if (bindDiscipline == null)
            {
                return NotFound();
            }

            _context.BindMainDisciplines.Remove(bindDiscipline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BindMainDisciplineExists(int id)
        {
            return _context.BindMainDisciplines.Any(e => e.IdBindMainDisciplines == id);
        }
    }
} 