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
        private readonly IStudyFormService _studyFormService;
        private readonly ITypeOfDisciplineService _typeOfDisciplineService;
        private readonly ICatalogYearService _catalogYearService;

        public ParametersController(INormativeService service,
                                        IEducationalDegreeService educationalDegreeService,
                                        IEducationStatusService educationStatusService,
                                        INotificationTemplateService notificationTemplateService,
                                        IStudyFormService studyFormService,
                                        ITypeOfDisciplineService typeOfDisciplineService,
                                        ICatalogYearService catalogYearService)
        {
            _normativeService = service;
            _educationStatusService = educationStatusService;
            _educationalDegreeService = educationalDegreeService;
            _notificationTemplateService = notificationTemplateService;
            _studyFormService = studyFormService;
            _typeOfDisciplineService = typeOfDisciplineService;
            _catalogYearService = catalogYearService;
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

        // -- StudyForm --
        [HttpGet("StudyForms")]
        public async Task<ActionResult<IEnumerable<StudyFormDto>>> GetStudyForms()
        {
            var result = await _studyFormService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("StudyForm/{id}")]
        public async Task<ActionResult<StudyFormDto>> GetStudyForm(int id)
        {
            var result = await _studyFormService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateStudyForm")]
        public async Task<ActionResult<StudyFormDto>> CreateStudyForm([FromBody] StudyFormDto dto)
        {
            var resultDto = await _studyFormService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetStudyForm), new { id = resultDto.IdStudyForm }, resultDto);
        }

        [HttpPut("UpdateStudyForm/{id}")]
        public async Task<IActionResult> UpdateStudyForm(int id, [FromBody] StudyFormDto dto)
        {
            var (success, statusCode, errorMessage) = await _studyFormService.UpdateAsync(id, dto);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

        [HttpDelete("DeleteStudyForm/{id}")]
        public async Task<IActionResult> DeleteStudyForm(int id)
        {
            var (success, statusCode, errorMessage) = await _studyFormService.DeleteAsync(id);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }


        // -- TypeOfDiscipline --

        [HttpGet("TypeOfDisciplines")]
        public async Task<ActionResult<IEnumerable<TypeOfDisciplineDto>>> GetTypeOfDisciplines()
        {
            var result = await _typeOfDisciplineService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("TypeOfDiscipline/{id}")]
        public async Task<ActionResult<TypeOfDisciplineDto>> GetTypeOfDiscipline(int id)
        {
            var result = await _typeOfDisciplineService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateTypeOfDiscipline")]
        public async Task<ActionResult<TypeOfDisciplineDto>> CreateTypeOfDiscipline(CreateTypeOfDisciplineDto dto)
        {
            var resultDto = await _typeOfDisciplineService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetTypeOfDiscipline), new { id = resultDto.IdTypeOfDiscipline }, resultDto);
        }

        [HttpPut("UpdateTypeOfDiscipline/{id}")]
        public async Task<IActionResult> UpdateTypeOfDiscipline(int id, TypeOfDisciplineDto dto)
        {
            var (success, statusCode, errorMessage) = await _typeOfDisciplineService.UpdateAsync(id, dto);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

        // -- CatalogYear --

        [HttpGet("CatalogYears")]
        public async Task<ActionResult<IEnumerable<CatalogYearDto>>> GetCatalogYears()
        {
            var result = await _catalogYearService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("CatalogYear/{id}")]
        public async Task<ActionResult<CatalogYearDto>> GetCatalogYear(int id)
        {
            var result = await _catalogYearService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateCatalogYear")]
        public async Task<ActionResult<CatalogYearDto>> CreateCatalogYear(CreateCatalogYearDto dto)
        {
            var resultDto = await _catalogYearService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCatalogYear), new { id = resultDto.IdCatalogYear }, resultDto);
        }

        [HttpPut("UpdateCatalogYear/{id}")]
        public async Task<IActionResult> UpdateCatalogYear(int id, UpdateCatalogYearDto dto)
        {
            var (success, statusCode, errorMessage) = await _catalogYearService.UpdateAsync(id, dto);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

        [HttpDelete("DeleteCatalogYear/{id}")]
        public async Task<IActionResult> DeleteCatalogYear(int id)
        {
            var (success, statusCode, errorMessage) = await _catalogYearService.DeleteAsync(id);
            if (!success)
                return StatusCode(statusCode, new { message = errorMessage });
            return NoContent();
        }

    }
}