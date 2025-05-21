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
    public class StudyFormController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudyFormController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StudyForm
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudyForm>>> GetStudyForms()
        {
            return await _context.StudyForms
                .Include(sf => sf.Students)
                .ToListAsync();
        }

        // GET: api/StudyForm/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudyForm>> GetStudyForm(int id)
        {
            var studyForm = await _context.StudyForms
                .Include(sf => sf.Students)
                .FirstOrDefaultAsync(sf => sf.IdStudyForm == id);

            if (studyForm == null)
            {
                return NotFound();
            }

            return studyForm;
        }

        // POST: api/StudyForm
        [HttpPost]
        public async Task<ActionResult<StudyForm>> CreateStudyForm(StudyForm studyForm)
        {
            _context.StudyForms.Add(studyForm);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudyForm), new { id = studyForm.IdStudyForm }, studyForm);
        }

        // PUT: api/StudyForm/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudyForm(int id, StudyForm studyForm)
        {
            if (id != studyForm.IdStudyForm)
            {
                return BadRequest();
            }

            _context.Entry(studyForm).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudyFormExists(id))
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

        // DELETE: api/StudyForm/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyForm(int id)
        {
            var studyForm = await _context.StudyForms.FindAsync(id);
            if (studyForm == null)
            {
                return NotFound();
            }

            _context.StudyForms.Remove(studyForm);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudyFormExists(int id)
        {
            return _context.StudyForms.Any(e => e.IdStudyForm == id);
        }
    }
} 