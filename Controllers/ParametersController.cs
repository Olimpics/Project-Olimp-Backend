using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //parametres
    public class ParametersController : ControllerBase
    {
        private readonly INormativeService _normativeService;
        private readonly IEducationStatusService _educationStatusService;
        private readonly IEducationalDegreeService _educationalDegreeService;
        private readonly INotificationTemplateService _notificationTemplateService;

        public ParametersController(INormativeService service,
                                        IEducationalDegreeService educationalDegreeService,
                                        IEducationStatusService educationStatusService,
                                        INotificationTemplateService notificationTemplateService)
        {
            _normativeService = service;
            _educationStatusService = educationStatusService;
            _educationalDegreeService = educationalDegreeService;
            _notificationTemplateService = notificationTemplateService;
        }


        // -- NORMATIVE --
        [HttpGet("Normatives")]
        public async Task<ActionResult<IEnumerable<NormativeDto>>> GetNormatives()
        {
            var result = await _normativeService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("Normative/{id}")]
        public async Task<ActionResult<NormativeDto>> GetNormative(int id)
        {
            var result = await _normativeService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("CreateNormative")]
        public async Task<ActionResult<NormativeDto>> CreateNormative(CreateNormativeDto dto)
        {
            var resultDto = await _normativeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetNormative), new { id = resultDto.IdNormative }, resultDto);
        }

        [HttpPut("UpdateNormative/{id}")]
        public async Task<IActionResult> UpdateNormative(int id, UpdateNormativeDto dto)
        {
            var (success, statusCode, errorMessage) = await _normativeService.UpdateAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });

            return NoContent();
        }

        [HttpDelete("DeleteNormative/{id}")]
        public async Task<IActionResult> DeleteNormative(int id)
        {
            var (success, statusCode, errorMessage) = await _normativeService.DeleteAsync(id);

            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });

            return NoContent();
        }


        // -- EDUCATION STATUS --
        [HttpGet("EducationStatuses")]
        public async Task<ActionResult<IEnumerable<EducationStatusDto>>> GetEducationStatuses()
        {
            var result = await _educationStatusService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("EducationStatus/{id}")]
        public async Task<ActionResult<EducationStatusDto>> GetEducationStatus(int id)
        {
            var result = await _educationStatusService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateEducationStatus")]
        public async Task<ActionResult<EducationStatusDto>> CreateEducationStatus(EducationStatusDto statusDto)
        {
            var resultDto = await _educationStatusService.CreateAsync(statusDto);
            return CreatedAtAction(nameof(GetEducationStatus), new { id = resultDto.IdEducationStatus }, resultDto);
        }

        [HttpPut("UpdateEducationStatus/{id}")]
        public async Task<IActionResult> UpdateEducationStatus(int id, EducationStatusDto statusDto)
        {
            var (success, statusCode, errorMessage) = await _educationStatusService.UpdateAsync(id, statusDto);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

        [HttpDelete("DeleteEducationStatus/{id}")]
        public async Task<IActionResult> DeleteEducationStatus(int id)
        {
            var (success, statusCode, errorMessage) = await _educationStatusService.DeleteAsync(id);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

        // -- EDUCATIONAL DEGREE --
        [HttpGet("EducationalDegrees")]
        public async Task<ActionResult<IEnumerable<EducationalDegreeDto>>> GetEducationalDegrees()
        {
            var result = await _educationalDegreeService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("EducationalDegree/{id}")]
        public async Task<ActionResult<EducationalDegreeDto>> GetEducationalDegree(int id)
        {
            var result = await _educationalDegreeService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateEducationalDegree")]
        public async Task<ActionResult<EducationalDegreeDto>> CreateEducationalDegree(CreateEducationalDegreeDto dto)
        {
            var resultDto = await _educationalDegreeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetEducationalDegree), new { id = resultDto.IdEducationalDegree }, resultDto);
        }

        [HttpPut("UpdateEducationalDegree/{id}")]
        public async Task<IActionResult> UpdateEducationalDegree(int id, UpdateEducationalDegreeDto dto)
        {
            var (success, statusCode, errorMessage) = await _educationalDegreeService.UpdateAsync(id, dto);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

        [HttpDelete("DeleteEducationalDegree/{id}")]
        public async Task<IActionResult> DeleteEducationalDegree(int id)
        {
            var (success, statusCode, errorMessage) = await _educationalDegreeService.DeleteAsync(id);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }


        // -- NOTIFICATION TEMPLATE --
        [HttpGet("NotificationTemplates")]
        public async Task<ActionResult<IEnumerable<NotificationTemplateDto>>> GetNotificationTemplates()
        {
            var result = await _notificationTemplateService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("NotificationTemplate/{id}")]
        public async Task<ActionResult<NotificationTemplateDto>> GetNotificationTemplate(int id)
        {
            var result = await _notificationTemplateService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateNotificationTemplate")]
        public async Task<ActionResult<NotificationTemplateDto>> CreateNotificationTemplate(CreateNotificationTemplateDto dto)
        {
            var resultDto = await _notificationTemplateService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetNotificationTemplate), new { id = resultDto.IdNotificationTemplates }, resultDto);
        }

        [HttpPut("UpdateNotificationTemplate/{id}")]
        public async Task<IActionResult> UpdateNotificationTemplate(int id, UpdateNotificationTemplateDto dto)
        {
            var (success, statusCode, errorMessage) = await _notificationTemplateService.UpdateAsync(id, dto);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

        /*
        [HttpDelete("DeleteNotificationTemplate/{id}")]
        public async Task<IActionResult> DeleteNotificationTemplate(int id)
        {
            var (success, statusCode, errorMessage) = await _notificationTemplateService.DeleteAsync(id);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }*/
    }
}