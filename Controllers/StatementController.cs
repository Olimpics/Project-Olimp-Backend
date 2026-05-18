using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlimpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatementController : ControllerBase
{
    private readonly IStatementService _statementService;

    public StatementController(IStatementService statementService)
    {
        _statementService = statementService;
    }

    [HttpGet("groups/main/{disciplineId}")]
    public async Task<ActionResult<List<GroupShortDto>>> GetGroupsByMainDiscipline(Guid disciplineId)
    {
        var groups = await _statementService.GetGroupsByMainDisciplineIdAsync(disciplineId);
        return Ok(groups);
    }

    [HttpGet("groups/selective/{disciplineId}")]
    public async Task<ActionResult<List<GroupShortDto>>> GetGroupsBySelectiveDiscipline(Guid disciplineId)
    {
        var groups = await _statementService.GetGroupsBySelectiveDisciplineIdAsync(disciplineId);
        return Ok(groups);
    }

    [HttpPost("generate/main/{disciplineId}")]
    public async Task<ActionResult<List<StatementFileDto>>> GenerateMainDisciplineStatements(Guid disciplineId)
    {
        try
        {
            var files = await _statementService.GenerateMainDisciplineStatementsAsync(disciplineId);
            return Ok(files);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("generate/selective/{disciplineId}")]
    public async Task<ActionResult<List<StatementFileDto>>> GenerateSelectiveDisciplineStatements(Guid disciplineId)
    {
        try
        {
            var files = await _statementService.GenerateSelectiveDisciplineStatementsAsync(disciplineId);
            return Ok(files);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
