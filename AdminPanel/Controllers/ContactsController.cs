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
public class ContactsController(ApplicationDbContext db) : ControllerBase
{
    [HttpPost("projects/{projectId:int}/contacts")]
    public async Task<ActionResult<ProjectContactDto>> Create(int projectId, ContactUpsertRequest request)
    {
        var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return NotFound("Project not found.");

        var contact = new ProjectContact
        {
            ProjectId = projectId,
            Name = request.Name,
            Role = request.Role,
            Email = request.Email,
            Phone = request.Phone,
            PreferredChannel = request.PreferredChannel,
            Notes = request.Notes
        };

        db.ProjectContacts.Add(contact);
        await db.SaveChangesAsync();

        return Ok(new ProjectContactDto(contact.Id, contact.Name, contact.Role, contact.Email, contact.Phone, contact.PreferredChannel, contact.Notes));
    }

    [HttpPut("contacts/{id:int}")]
    public async Task<ActionResult<ProjectContactDto>> Update(int id, ContactUpsertRequest request)
    {
        var contact = await db.ProjectContacts.FindAsync(id);
        if (contact is null) return NotFound();

        contact.Name = request.Name;
        contact.Role = request.Role;
        contact.Email = request.Email;
        contact.Phone = request.Phone;
        contact.PreferredChannel = request.PreferredChannel;
        contact.Notes = request.Notes;

        await db.SaveChangesAsync();

        return Ok(new ProjectContactDto(contact.Id, contact.Name, contact.Role, contact.Email, contact.Phone, contact.PreferredChannel, contact.Notes));
    }

    [HttpDelete("contacts/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var contact = await db.ProjectContacts.FindAsync(id);
        if (contact is null) return NotFound();

        db.ProjectContacts.Remove(contact);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
