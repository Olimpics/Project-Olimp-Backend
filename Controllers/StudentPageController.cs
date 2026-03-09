using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Controllers;
[Route("api/[controller]")]
[ApiController]
public class StudentPageController:ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentPageController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("disciplines/{studentId}")]
    public async Task<ActionResult<StudentDisciplinesDto>> GetStudentDisciplines(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.BindAddDisciplines)
                .ThenInclude(bad => bad.AddDisciplines)
            .Include(s => s.EducationalProgram)
                .ThenInclude(ep => ep.BindMainDisciplines)
            .FirstOrDefaultAsync(s => s.IdStudent == studentId);

        if (student == null)
        {
            return NotFound();
        }

        var result = new StudentDisciplinesDto
        {
            StudentId = student.IdStudent,
            StudentName = student.NameStudent,
            MainDisciplines = _mapper.Map<List<BindMainDisciplineDto>>(student.EducationalProgram.BindMainDisciplines),
            AdditionalDisciplines = _mapper.Map<List<BindAddDisciplineDto>>(student.BindAddDisciplines)
        };

        return Ok(result);
    }
}