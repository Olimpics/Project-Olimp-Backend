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
    public class AddDetailController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddDetailController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddDetailDto>>> GetAddDetails()
        {
            var details = await _context.AddDetails
                .Include(d => d.Department)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<AddDetailDto>>(details));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AddDetailDto>> GetAddDetail(int id)
        {
            var detail = await _context.AddDetails
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.IdAddDetails == id);

            if (detail == null)
                return NotFound();

            return Ok(_mapper.Map<AddDetailDto>(detail));
        }

        [HttpPost]
        public async Task<ActionResult<AddDetailDto>> CreateAddDetail(CreateAddDetailDto dto)
        {
            var detail = _mapper.Map<AddDetail>(dto);
            _context.AddDetails.Add(detail);
            await _context.SaveChangesAsync();

            var resultDto = await GetAddDetailWithIncludes(detail.IdAddDetails);
            return CreatedAtAction(nameof(GetAddDetail), new { id = detail.IdAddDetails }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddDetail(int id, UpdateAddDetailDto dto)
        {
            if (id != dto.IdAddDetails)
                return BadRequest();

            var detail = await _context.AddDetails.FindAsync(id);
            if (detail == null)
                return NotFound();

            _mapper.Map(dto, detail);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AddDetailExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddDetail(int id)
        {
            var detail = await _context.AddDetails.FindAsync(id);
            if (detail == null)
                return NotFound();

            _context.AddDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<AddDetailDto> GetAddDetailWithIncludes(int id)
        {
            var detail = await _context.AddDetails
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.IdAddDetails == id);

            return _mapper.Map<AddDetailDto>(detail);
        }

        private bool AddDetailExists(int id)
        {
            return _context.AddDetails.Any(d => d.IdAddDetails == id);
        }
    }
} 