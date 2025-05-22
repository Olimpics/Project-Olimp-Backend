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
    public class EducationalDegreeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EducationalDegreeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EducationalDegree
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationalDegree>>> GetEducationalDegrees()
        {
            return await _context.EducationalDegrees
                .Include(ed => ed.Students)
                .ToListAsync();
        }

        // GET: api/EducationalDegree/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EducationalDegree>> GetEducationalDegree(int id)
        {
            var degree = await _context.EducationalDegrees
                .Include(ed => ed.Students)
                .FirstOrDefaultAsync(ed => ed.IdEducationalDegree == id);

            if (degree == null)
            {
                return NotFound();
            }

            return degree;
        }

        // POST: api/EducationalDegree
        [HttpPost]
        public async Task<ActionResult<EducationalDegree>> CreateEducationalDegree(EducationalDegree degree)
        {
            _context.EducationalDegrees.Add(degree);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEducationalDegree), new { id = degree.IdEducationalDegree }, degree);
        }

        // PUT: api/EducationalDegree/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationalDegree(int id, EducationalDegree degree)
        {
            if (id != degree.IdEducationalDegree)
            {
                return BadRequest();
            }

            _context.Entry(degree).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationalDegreeExists(id))
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

        // DELETE: api/EducationalDegree/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationalDegree(int id)
        {
            var degree = await _context.EducationalDegrees.FindAsync(id);
            if (degree == null)
            {
                return NotFound();
            }

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