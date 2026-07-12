using AdminPanel.Client.Contracts;
using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectSummaryDto>>> GetAll()
    {
        var projects = await db.Projects
            .OrderByDescending(p => p.UpdatedAt)
            .Select(p => new ProjectSummaryDto(
                p.Id, p.CompanyName, p.ProjectTitle, p.Status.ToString(), p.Currency,
                p.RateAmount, p.RateType.ToString(), p.StartDate, p.EndDate))
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDetailDto>> GetById(int id)
    {
        var project = await db.Projects
            .Include(p => p.Contacts)
            .Include(p => p.Documents)
            .Include(p => p.Invoices)
            .Include(p => p.TimelineNotes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null) return NotFound();

        return Ok(ToDetailDto(project));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDetailDto>> Create(ProjectUpsertRequest request)
    {
        if (!TryParseEnums(request, out var status, out var rateType, out var error))
            return BadRequest(error);

        var project = new Project
        {
            CompanyName = request.CompanyName,
            ProjectTitle = request.ProjectTitle,
            Description = request.Description,
            Status = status,
            Source = request.Source,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Currency = request.Currency,
            RateType = rateType,
            RateAmount = request.RateAmount,
            BudgetTotal = request.BudgetTotal,
            PaymentTerms = request.PaymentTerms,
            Priority = request.Priority,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, ToDetailDto(project));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDetailDto>> Update(int id, ProjectUpsertRequest request)
    {
        if (!TryParseEnums(request, out var status, out var rateType, out var error))
            return BadRequest(error);

        var project = await db.Projects
            .Include(p => p.Contacts)
            .Include(p => p.Documents)
            .Include(p => p.Invoices)
            .Include(p => p.TimelineNotes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null) return NotFound();

        project.CompanyName = request.CompanyName;
        project.ProjectTitle = request.ProjectTitle;
        project.Description = request.Description;
        project.Status = status;
        project.Source = request.Source;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.Currency = request.Currency;
        project.RateType = rateType;
        project.RateAmount = request.RateAmount;
        project.BudgetTotal = request.BudgetTotal;
        project.PaymentTerms = request.PaymentTerms;
        project.Priority = request.Priority;
        project.Notes = request.Notes;
        project.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(ToDetailDto(project));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return NotFound();

        db.Projects.Remove(project);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static bool TryParseEnums(ProjectUpsertRequest request, out ProjectStatus status, out RateType rateType, out string? error)
    {
        error = null;
        if (!Enum.TryParse(request.Status, ignoreCase: true, out status))
        {
            error = $"Invalid status '{request.Status}'.";
        }
        if (!Enum.TryParse(request.RateType, ignoreCase: true, out rateType))
        {
            error = $"Invalid rate type '{request.RateType}'.";
        }
        return error is null;
    }

    internal static ProjectDetailDto ToDetailDto(Project p) => new(
        p.Id, p.CompanyName, p.ProjectTitle, p.Description, p.Status.ToString(), p.Source,
        p.StartDate, p.EndDate, p.Currency, p.RateType.ToString(), p.RateAmount, p.BudgetTotal,
        p.PaymentTerms, p.Priority, p.Notes, p.CreatedAt, p.UpdatedAt,
        p.Contacts.Select(c => new ProjectContactDto(c.Id, c.Name, c.Role, c.Email, c.Phone, c.PreferredChannel, c.Notes)).ToList(),
        p.Documents.Select(d => new ProjectDocumentDto(d.Id, d.Type.ToString(), d.Title, d.MimeType, d.Size, d.UploadedAt)).ToList(),
        p.Invoices.Select(i => new ProjectInvoiceDto(i.Id, i.Amount, i.Currency, i.IssuedDate, i.DueDate, i.PaidAt, i.Status.ToString())).ToList(),
        p.TimelineNotes.OrderByDescending(n => n.CreatedAt).Select(n => new ProjectNoteDto(n.Id, n.Body, n.CreatedAt)).ToList()
    );
}
