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
    public class EducationalProgramController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EducationalProgramController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EducationalProgram
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationalProgram>>> GetEducationalPrograms()
        {
            return await _context.EducationalPrograms
                .Include(ep => ep.Students)
                .ToListAsync();
        }

        // GET: api/EducationalProgram/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EducationalProgram>> GetEducationalProgram(int id)
        {
            var program = await _context.EducationalPrograms
                .Include(ep => ep.Students)
                .FirstOrDefaultAsync(ep => ep.IdEducationalProgram == id);

            if (program == null)
            {
                return NotFound();
            }

            return program;
        }

        // POST: api/EducationalProgram
        [HttpPost]
        public async Task<ActionResult<EducationalProgram>> CreateEducationalProgram(EducationalProgram program)
        {
            _context.EducationalPrograms.Add(program);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEducationalProgram), new { id = program.IdEducationalProgram }, program);
        }

        // PUT: api/EducationalProgram/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationalProgram(int id, EducationalProgram program)
        {
            if (id != program.IdEducationalProgram)
            {
                return BadRequest();
            }

            _context.Entry(program).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationalProgramExists(id))
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

        // DELETE: api/EducationalProgram/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationalProgram(int id)
        {
            var program = await _context.EducationalPrograms.FindAsync(id);
            if (program == null)
            {
                return NotFound();
            }

            _context.EducationalPrograms.Remove(program);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EducationalProgramExists(int id)
        {
            return _context.EducationalPrograms.Any(e => e.IdEducationalProgram == id);
        }
    }
} 