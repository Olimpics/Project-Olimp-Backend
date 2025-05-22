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
    public class EducationStatusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EducationStatusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EducationStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationStatus>>> GetEducationStatuses()
        {
            return await _context.EducationStatuses
                .Include(es => es.Students)
                .ToListAsync();
        }

        // GET: api/EducationStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EducationStatus>> GetEducationStatus(int id)
        {
            var status = await _context.EducationStatuses
                .Include(es => es.Students)
                .FirstOrDefaultAsync(es => es.IdEducationStatus == id);

            if (status == null)
            {
                return NotFound();
            }

            return status;
        }

        // POST: api/EducationStatus
        [HttpPost]
        public async Task<ActionResult<EducationStatus>> CreateEducationStatus(EducationStatus status)
        {
            _context.EducationStatuses.Add(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEducationStatus), new { id = status.IdEducationStatus }, status);
        }

        // PUT: api/EducationStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationStatus(int id, EducationStatus status)
        {
            if (id != status.IdEducationStatus)
            {
                return BadRequest();
            }

            _context.Entry(status).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationStatusExists(id))
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

        // DELETE: api/EducationStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationStatus(int id)
        {
            var status = await _context.EducationStatuses.FindAsync(id);
            if (status == null)
            {
                return NotFound();
            }

            _context.EducationStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EducationStatusExists(int id)
        {
            return _context.EducationStatuses.Any(e => e.IdEducationStatus == id);
        }
    }
} 