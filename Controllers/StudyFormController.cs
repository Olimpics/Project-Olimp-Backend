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
    public class StudyFormController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudyFormController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/StudyForm
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudyFormDto>>> GetStudyForms()
        {
            var forms = await _context.StudyForms.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<StudyFormDto>>(forms));
        }

        // GET: api/StudyForm/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudyFormDto>> GetStudyForm(int id)
        {
            var form = await _context.StudyForms.FindAsync(id);
            if (form == null)
                return NotFound();

            return Ok(_mapper.Map<StudyFormDto>(form));
        }

        // POST: api/StudyForm
        [HttpPost]
        public async Task<ActionResult<StudyFormDto>> CreateStudyForm(StudyFormDto formDto)
        {
            var form = _mapper.Map<StudyForm>(formDto);
            _context.StudyForms.Add(form);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<StudyFormDto>(form);
            return CreatedAtAction(nameof(GetStudyForm), new { id = form.IdStudyForm }, resultDto);
        }

        // PUT: api/StudyForm/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudyForm(int id, StudyFormDto formDto)
        {
            if (id != formDto.IdStudyForm)
                return BadRequest();

            var form = await _context.StudyForms.FindAsync(id);
            if (form == null)
                return NotFound();

            _mapper.Map(formDto, form);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/StudyForm/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyForm(int id)
        {
            var form = await _context.StudyForms.FindAsync(id);
            if (form == null)
                return NotFound();

            _context.StudyForms.Remove(form);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudyFormExists(int id)
        {
            return _context.StudyForms.Any(e => e.IdStudyForm == id);
        }
    }

}