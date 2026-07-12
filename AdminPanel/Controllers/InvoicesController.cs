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
public class InvoicesController(ApplicationDbContext db) : ControllerBase
{
    [HttpPost("projects/{projectId:int}/invoices")]
    public async Task<ActionResult<ProjectInvoiceDto>> Create(int projectId, InvoiceUpsertRequest request)
    {
        var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return NotFound("Project not found.");

        if (!Enum.TryParse<InvoiceStatus>(request.Status, ignoreCase: true, out var status))
            return BadRequest($"Invalid status '{request.Status}'.");

        var invoice = new ProjectInvoice
        {
            ProjectId = projectId,
            Amount = request.Amount,
            Currency = request.Currency,
            IssuedDate = request.IssuedDate,
            DueDate = request.DueDate,
            PaidAt = request.PaidAt,
            Status = status
        };

        db.ProjectInvoices.Add(invoice);
        await db.SaveChangesAsync();

        return Ok(ToDto(invoice));
    }

    [HttpPut("invoices/{id:int}")]
    public async Task<ActionResult<ProjectInvoiceDto>> Update(int id, InvoiceUpsertRequest request)
    {
        var invoice = await db.ProjectInvoices.FindAsync(id);
        if (invoice is null) return NotFound();

        if (!Enum.TryParse<InvoiceStatus>(request.Status, ignoreCase: true, out var status))
            return BadRequest($"Invalid status '{request.Status}'.");

        invoice.Amount = request.Amount;
        invoice.Currency = request.Currency;
        invoice.IssuedDate = request.IssuedDate;
        invoice.DueDate = request.DueDate;
        invoice.PaidAt = request.PaidAt;
        invoice.Status = status;

        await db.SaveChangesAsync();

        return Ok(ToDto(invoice));
    }

    [HttpDelete("invoices/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var invoice = await db.ProjectInvoices.FindAsync(id);
        if (invoice is null) return NotFound();

        db.ProjectInvoices.Remove(invoice);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static ProjectInvoiceDto ToDto(ProjectInvoice i) =>
        new(i.Id, i.Amount, i.Currency, i.IssuedDate, i.DueDate, i.PaidAt, i.Status.ToString());
}
