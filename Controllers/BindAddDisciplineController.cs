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
    public class BindAddDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BindAddDisciplineController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BindAddDiscipline
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BindAddDiscipline>>> GetBindAddDisciplines()
        {
            return await _context.BindAddDisciplines
                .Include(bad => bad.Student)
                .Include(bad => bad.AddDisciplinesId)
                .ToListAsync();
        }

        // GET: api/BindAddDiscipline/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BindAddDiscipline>> GetBindAddDiscipline(int id)
        {
            var bindDiscipline = await _context.BindAddDisciplines
                .Include(bad => bad.Student)
                .Include(bad => bad.AddDisciplinesId)
                .FirstOrDefaultAsync(bad => bad.IdBindAddDisciplines == id);

            if (bindDiscipline == null)
            {
                return NotFound();
            }

            return bindDiscipline;
        }

        // POST: api/BindAddDiscipline
        [HttpPost]
        public async Task<ActionResult<BindAddDiscipline>> CreateBindAddDiscipline(BindAddDiscipline bindDiscipline)
        {
            _context.BindAddDisciplines.Add(bindDiscipline);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBindAddDiscipline), new { id = bindDiscipline.IdBindAddDisciplines }, bindDiscipline);
        }

        // PUT: api/BindAddDiscipline/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindAddDiscipline(int id, BindAddDiscipline bindDiscipline)
        {
            if (id != bindDiscipline.IdBindAddDisciplines)
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
            var bindDiscipline = await _context.BindAddDisciplines.FindAsync(id);
            if (bindDiscipline == null)
            {
                return NotFound();
            }

            _context.BindAddDisciplines.Remove(bindDiscipline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BindAddDisciplineExists(int id)
        {
            return _context.BindAddDisciplines.Any(e => e.IdBindAddDisciplines == id);
        }
    }
} 