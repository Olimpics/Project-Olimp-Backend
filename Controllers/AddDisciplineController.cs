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
    public class AddDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddDisciplineController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AddDiscipline
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddDiscipline>>> GetAddDisciplines()
        {
            return await _context.AddDisciplines
                .ToListAsync();
        }

        // GET: api/AddDiscipline/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddDiscipline>> GetAddDiscipline(int id)
        {
            var discipline = await _context.AddDisciplines
                .FirstOrDefaultAsync(ad => ad.idAddDisciplines == id);

            if (discipline == null)
            {
                return NotFound();
            }

            return discipline;
        }

        // POST: api/AddDiscipline
        [HttpPost]
        public async Task<ActionResult<AddDiscipline>> CreateAddDiscipline(AddDiscipline discipline)
        {
            _context.AddDisciplines.Add(discipline);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddDiscipline), new { id = discipline.idAddDisciplines }, discipline);
        }

        // PUT: api/AddDiscipline/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddDiscipline(int id, AddDiscipline discipline)
        {
            if (id != discipline.idAddDisciplines)
            {
                return BadRequest();
            }

            _context.Entry(discipline).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddDisciplineExists(id))
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

        // DELETE: api/AddDiscipline/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddDiscipline(int id)
        {
            var discipline = await _context.AddDisciplines.FindAsync(id);
            if (discipline == null)
            {
                return NotFound();
            }

            _context.AddDisciplines.Remove(discipline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddDisciplineExists(int id)
        {
            return _context.AddDisciplines.Any(e => e.idAddDisciplines == id);
        }
    }
} 