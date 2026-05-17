using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.Services;
using System;
using System.Threading.Tasks;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SimilarityController : ControllerBase
{
    private readonly ISimilarityService _similarityService;
    private readonly IEpSimilarityService _epSimilarityService;

    public SimilarityController(ISimilarityService similarityService, IEpSimilarityService epSimilarityService)
    {
        _similarityService = similarityService;
        _epSimilarityService = epSimilarityService;
    }

    [HttpPost("disciplines/generate-keys")]
    public async Task<IActionResult> GenerateDisciplineKeys()
    {
        await _similarityService.GenerateKeysForAllAsync();
        return Ok(new { message = "Keys generated successfully for all disciplines" });
    }

    [HttpPost("disciplines/form-groups")]
    public async Task<IActionResult> FormDisciplineGroups()
    {
        await _similarityService.FormGroupsAsync();
        return Ok(new { message = "Discipline similarity groups formed successfully" });
    }

    [HttpPost("disciplines/process-new")]
    public async Task<IActionResult> ProcessNewDisciplines()
    {
        await _similarityService.ProcessNewDisciplinesAsync();
        return Ok(new { message = "New disciplines processed and added to similarity groups" });
    }

    [HttpGet("disciplines/similar/{id}")]
    public async Task<IActionResult> GetSimilarDisciplines(Guid id)
    {
        var result = await _similarityService.GetSimilarByDisciplineIdAsync(id);
        return Ok(result);
    }

    [HttpDelete("disciplines/group/{groupId}/remove/{disciplineId}")]
    public async Task<IActionResult> RemoveDisciplineFromGroup(Guid groupId, Guid disciplineId)
    {
        await _similarityService.RemoveDisciplineFromGroupAsync(groupId, disciplineId);
        return Ok(new { message = "Discipline removed from similarity group" });
    }

    [HttpPost("ep/generate-keys")]
    public async Task<IActionResult> GenerateEpKeys()
    {
        await _epSimilarityService.GenerateKeysForAllAsync();
        return Ok(new { message = "Keys generated successfully for all educational programs" });
    }

    [HttpPost("ep/form-groups")]
    public async Task<IActionResult> FormEpGroups()
    {
        await _epSimilarityService.FormGroupsAsync();
        return Ok(new { message = "Educational program similarity groups formed successfully" });
    }

    [HttpPost("ep/process-new")]
    public async Task<IActionResult> ProcessNewEp()
    {
        await _epSimilarityService.ProcessNewProgramsAsync();
        return Ok(new { message = "New educational programs processed and added to similarity groups" });
    }

    [HttpGet("ep/similar/{id}")]
    public async Task<IActionResult> GetSimilarEp(Guid id)
    {
        var result = await _epSimilarityService.GetSimilarByEpIdAsync(id);
        return Ok(result);
    }

    [HttpDelete("ep/group/{groupId}/remove/{epId}")]
    public async Task<IActionResult> RemoveEpFromGroup(Guid groupId, Guid epId)
    {
        await _epSimilarityService.RemoveEpFromGroupAsync(groupId, epId);
        return Ok(new { message = "Educational program removed from similarity group" });
    }
}
