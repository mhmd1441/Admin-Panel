using AdminPanel.Client.Contracts;
using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class DocumentsController(ApplicationDbContext db, SupabaseStorageService storage) : ControllerBase
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".png", ".jpg", ".jpeg", ".webp", ".txt", ".csv", ".zip"
    };

    [HttpPost("projects/{projectId:int}/documents")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<ProjectDocumentDto>> Upload(
        int projectId, [FromForm] IFormFile file, [FromForm] string type, [FromForm] string? title)
    {
        var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return NotFound("Project not found.");

        if (file.Length == 0) return BadRequest("File is empty.");
        if (!Enum.TryParse<DocumentType>(type, ignoreCase: true, out var docType))
            return BadRequest($"Invalid document type '{type}'.");

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
            return BadRequest($"File type '{extension}' is not allowed. Allowed: {string.Join(", ", AllowedExtensions)}");

        var safeFileName = SanitizeFileName(Path.GetFileName(file.FileName));
        var storagePath = $"{projectId}/{Guid.NewGuid()}-{safeFileName}";

        await using (var stream = file.OpenReadStream())
        {
            await storage.UploadAsync(storagePath, stream, file.ContentType);
        }

        var document = new ProjectDocument
        {
            ProjectId = projectId,
            Type = docType,
            Title = Truncate(string.IsNullOrWhiteSpace(title) ? safeFileName : title, 200),
            StoragePath = storagePath,
            MimeType = file.ContentType,
            Size = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        db.ProjectDocuments.Add(document);
        await db.SaveChangesAsync();

        return Ok(new ProjectDocumentDto(document.Id, document.Type.ToString(), document.Title, document.MimeType, document.Size, document.UploadedAt));
    }

    [HttpGet("documents/{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var document = await db.ProjectDocuments.FindAsync(id);
        if (document is null) return NotFound();

        var signedUrl = await storage.CreateSignedUrlAsync(document.StoragePath);
        return Redirect(signedUrl);
    }

    [HttpDelete("documents/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var document = await db.ProjectDocuments.FindAsync(id);
        if (document is null) return NotFound();

        await storage.DeleteAsync(document.StoragePath);

        db.ProjectDocuments.Remove(document);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // Storage paths must contain only predictable characters: strips path separators,
    // "..", and anything outside [a-zA-Z0-9._-], and caps the length.
    private static string SanitizeFileName(string fileName)
    {
        var cleaned = new string(fileName.Select(c =>
            char.IsLetterOrDigit(c) || c is '.' or '_' or '-' ? c : '_').ToArray());

        while (cleaned.Contains(".."))
            cleaned = cleaned.Replace("..", ".");

        cleaned = cleaned.Trim('.', '_', '-');

        if (cleaned.Length > 100)
        {
            var ext = Path.GetExtension(cleaned);
            cleaned = cleaned[..(100 - ext.Length)] + ext;
        }

        return string.IsNullOrWhiteSpace(cleaned) ? "file" : cleaned;
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}
