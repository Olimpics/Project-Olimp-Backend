using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Data;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HierarchyController : ControllerBase
{
    private readonly AppDbContext _context;

    public HierarchyController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("trees")]
    [RequirePermission(RbacPermissions.UsersRead)]
    public async Task<ActionResult<IReadOnlyList<HierarchyTreeDto>>> GetTrees()
    {
        var trees = await _context.HierarchyTrees
            .AsNoTracking()
            .OrderBy(tree => tree.Name)
            .Select(tree => new HierarchyTreeDto
            {
                IdTree = tree.IdTree,
                Code = tree.Code,
                Name = tree.Name
            })
            .ToListAsync();

        return Ok(trees);
    }

    [HttpGet("trees/{treeId:guid}/nodes")]
    [RequirePermission(RbacPermissions.UsersRead)]
    public async Task<ActionResult<IReadOnlyList<HierarchyNodeDto>>> GetNodes(Guid treeId)
    {
        var nodes = await _context.HierarchyNodes
            .AsNoTracking()
            .Where(node => node.TreeId == treeId)
            .OrderByDescending(node => node.ManagementWeight)
            .ThenBy(node => node.Name)
            .Select(node => new HierarchyNodeDto
            {
                IdNode = node.IdNode,
                TreeId = node.TreeId,
                ParentId = node.ParentId,
                Code = node.Code,
                Name = node.Name,
                ManagementWeight = node.ManagementWeight
            })
            .ToListAsync();

        return Ok(nodes);
    }
}
