using AdminPanel.Client.Contracts;
using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class RequirementsController(ApplicationDbContext db) : ControllerBase
{
    [HttpPost("projects/{projectId:int}/requirements")]
    public async Task<ActionResult<ProjectRequirementDto>> Create(int projectId, RequirementUpsertRequest request)
    {
        var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return NotFound("Project not found.");

        var requirement = new ProjectRequirement
        {
            ProjectId = projectId,
            Description = request.Description,
            IsDone = request.IsDone,
            CreatedAt = DateTime.UtcNow
        };

        db.ProjectRequirements.Add(requirement);
        await db.SaveChangesAsync();

        return Ok(ToDto(requirement));
    }

    [HttpPut("requirements/{id:int}")]
    public async Task<ActionResult<ProjectRequirementDto>> Update(int id, RequirementUpsertRequest request)
    {
        var requirement = await db.ProjectRequirements.FindAsync(id);
        if (requirement is null) return NotFound();

        requirement.Description = request.Description;
        requirement.IsDone = request.IsDone;

        await db.SaveChangesAsync();

        return Ok(ToDto(requirement));
    }

    [HttpDelete("requirements/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var requirement = await db.ProjectRequirements.FindAsync(id);
        if (requirement is null) return NotFound();

        db.ProjectRequirements.Remove(requirement);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static ProjectRequirementDto ToDto(ProjectRequirement r) =>
        new(r.Id, r.Description, r.IsDone, r.CreatedAt);
}
