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
public class NotesController(ApplicationDbContext db) : ControllerBase
{
    [HttpPost("projects/{projectId:int}/notes")]
    public async Task<ActionResult<ProjectNoteDto>> Create(int projectId, NoteCreateRequest request)
    {
        var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return NotFound("Project not found.");

        var note = new ProjectNote
        {
            ProjectId = projectId,
            Body = request.Body,
            CreatedAt = DateTime.UtcNow
        };

        db.ProjectNotes.Add(note);
        await db.SaveChangesAsync();

        return Ok(new ProjectNoteDto(note.Id, note.Body, note.CreatedAt));
    }

    [HttpDelete("notes/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await db.ProjectNotes.FindAsync(id);
        if (note is null) return NotFound();

        db.ProjectNotes.Remove(note);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
