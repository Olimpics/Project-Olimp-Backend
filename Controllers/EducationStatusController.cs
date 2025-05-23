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
    public class EducationStatusController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EducationStatusController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationStatusDto>>> GetEducationStatuses()
        {
            var statuses = await _context.EducationStatuses.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<EducationStatusDto>>(statuses));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EducationStatusDto>> GetEducationStatus(int id)
        {
            var status = await _context.EducationStatuses.FindAsync(id);
            if (status == null)
                return NotFound();

            return Ok(_mapper.Map<EducationStatusDto>(status));
        }

        [HttpPost]
        public async Task<ActionResult<EducationStatusDto>> CreateEducationStatus(EducationStatusDto statusDto)
        {
            var status = _mapper.Map<EducationStatus>(statusDto);
            _context.EducationStatuses.Add(status);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<EducationStatusDto>(status);
            return CreatedAtAction(nameof(GetEducationStatus), new { id = status.IdEducationStatus }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationStatus(int id, EducationStatusDto statusDto)
        {
            if (id != statusDto.IdEducationStatus)
                return BadRequest();

            var status = await _context.EducationStatuses.FindAsync(id);
            if (status == null)
                return NotFound();

            _mapper.Map(statusDto, status);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationStatus(int id)
        {
            var status = await _context.EducationStatuses.FindAsync(id);
            if (status == null)
                return NotFound();

            _context.EducationStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}