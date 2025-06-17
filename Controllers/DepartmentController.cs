using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using System;
using System.Linq;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetDepartments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] int? facultyId = null,
            [FromQuery] string? search = null)
        {
            var query = _context.Departments
                .Include(d => d.Faculty)
                .AsQueryable();

            // Apply facultyId filter
            if (facultyId.HasValue)
            {
                query = query.Where(d => d.FacultyId == facultyId.Value);
            }

            // Apply search filter for both idDepartment and nameDepartment
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                
                // Check if search is a number (for ID search)
                if (int.TryParse(search.Trim(), out int searchId))
                {
                    // If search is a number, search by exact ID match
                    query = query.Where(d => d.IdDepartment == searchId);
                }
                else
                {
                    // If search is not a number, search by name and abbreviation
                    query = query.Where(d =>
                        EF.Functions.Like(d.NameDepartment.ToLower(), $"%{lowerSearch}%") ||
                        EF.Functions.Like(d.Abbreviation.ToLower(), $"%{lowerSearch}%"));
                }
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var departments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = departments.Select(d => _mapper.Map<DepartmentDto>(d)).ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Items = result
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Faculty)
                .FirstOrDefaultAsync(d => d.IdDepartment == id);

            if (department == null)
                return NotFound();

            return Ok(_mapper.Map<DepartmentDto>(department));
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var resultDto = await GetDepartmentWithIncludes(department.IdDepartment);
            return CreatedAtAction(nameof(GetDepartment), new { id = department.IdDepartment }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto dto)
        {
            if (id != dto.IdDepartment)
                return BadRequest();

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            _mapper.Map(dto, department);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!DepartmentExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<DepartmentDto> GetDepartmentWithIncludes(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Faculty)
                .FirstOrDefaultAsync(d => d.IdDepartment == id);

            return _mapper.Map<DepartmentDto>(department);
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.IdDepartment == id);
        }
    }
} 