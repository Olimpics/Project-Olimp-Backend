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
    public class NotificationTemplateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public NotificationTemplateController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationTemplateDto>>> GetNotificationTemplates()
        {
            var templates = await _context.NotificationTemplates.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<NotificationTemplateDto>>(templates));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationTemplateDto>> GetNotificationTemplate(int id)
        {
            var template = await _context.NotificationTemplates.FindAsync(id);

            if (template == null)
                return NotFound();

            return Ok(_mapper.Map<NotificationTemplateDto>(template));
        }

        [HttpPost]
        public async Task<ActionResult<NotificationTemplateDto>> CreateNotificationTemplate(CreateNotificationTemplateDto dto)
        {
            var template = _mapper.Map<NotificationTemplate>(dto);
            _context.NotificationTemplates.Add(template);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<NotificationTemplateDto>(template);
            return CreatedAtAction(nameof(GetNotificationTemplate), new { id = template.IdNotificationTemplates }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotificationTemplate(int id, UpdateNotificationTemplateDto dto)
        {
            var template = await _context.NotificationTemplates.FindAsync(id);
            if (template == null)
                return NotFound();

            _mapper.Map(dto, template);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!NotificationTemplateExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotificationTemplate(int id)
        {
            var template = await _context.NotificationTemplates.FindAsync(id);
            if (template == null)
                return NotFound();

            _context.NotificationTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotificationTemplateExists(int id)
        {
            return _context.NotificationTemplates.Any(e => e.IdNotificationTemplates == id);
        }
    }
} 