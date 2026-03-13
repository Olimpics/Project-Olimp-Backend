using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudyFormController : ControllerBase
    {
        private readonly IStudyFormService _service;

        public StudyFormController(IStudyFormService service)
        {
            _service = service;
        }

        // GET: api/StudyForm
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudyFormDto>>> GetStudyForms()
        {
            var forms = await _service.GetAllAsync();
            return Ok(forms);
        }

        // GET: api/StudyForm/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudyFormDto>> GetStudyForm(int id)
        {
            var form = await _service.GetByIdAsync(id);

            if (form == null)
                return NotFound();

            return Ok(form);
        }

        // POST: api/StudyForm
        [HttpPost]
        public async Task<ActionResult<StudyFormDto>> CreateStudyForm([FromBody] StudyFormDto formDto)
        {
            var resultDto = await _service.CreateAsync(formDto);

            return CreatedAtAction(nameof(GetStudyForm), new { id = resultDto.IdStudyForm }, resultDto);
        }

        // PUT: api/StudyForm/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudyForm(int id, [FromBody] StudyFormDto formDto)
        {
            var (success, statusCode, errorMessage) = await _service.UpdateAsync(id, formDto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        // DELETE: api/StudyForm/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyForm(int id)
        {
            var (success, statusCode, errorMessage) = await _service.DeleteAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
}