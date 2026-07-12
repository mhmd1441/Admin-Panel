using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Client.Contracts;

public record ProjectSummaryDto(
    int Id,
    string CompanyName,
    string ProjectTitle,
    string Status,
    string Currency,
    decimal? RateAmount,
    string RateType,
    DateOnly? StartDate,
    DateOnly? EndDate
);

public record ProjectDetailDto(
    int Id,
    string CompanyName,
    string ProjectTitle,
    string? Description,
    string Status,
    string? Source,
    DateOnly? StartDate,
    DateOnly? EndDate,
    string Currency,
    string RateType,
    decimal? RateAmount,
    decimal? BudgetTotal,
    string? PaymentTerms,
    string? Priority,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<ProjectContactDto> Contacts,
    List<ProjectDocumentDto> Documents,
    List<ProjectInvoiceDto> Invoices,
    List<ProjectNoteDto> TimelineNotes
);

public record ProjectContactDto(int Id, string Name, string? Role, string? Email, string? Phone, string? PreferredChannel, string? Notes);

public record ProjectDocumentDto(int Id, string Type, string Title, string MimeType, long Size, DateTime UploadedAt);

public record ProjectInvoiceDto(int Id, decimal Amount, string Currency, DateOnly IssuedDate, DateOnly? DueDate, DateTime? PaidAt, string Status);

public record ProjectNoteDto(int Id, string Body, DateTime CreatedAt);

public class ProjectUpsertRequest
{
    [Required, StringLength(200)] public string CompanyName { get; set; } = "";
    [Required, StringLength(200)] public string ProjectTitle { get; set; } = "";
    [StringLength(5000)] public string? Description { get; set; }
    [Required, StringLength(30)] public string Status { get; set; } = "Lead";
    [StringLength(200)] public string? Source { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [StringLength(10)] public string Currency { get; set; } = "USD";
    [StringLength(30)] public string RateType { get; set; } = "Hourly";
    [Range(0, 100_000_000)] public decimal? RateAmount { get; set; }
    [Range(0, 1_000_000_000)] public decimal? BudgetTotal { get; set; }
    [StringLength(500)] public string? PaymentTerms { get; set; }
    [StringLength(50)] public string? Priority { get; set; }
    [StringLength(10000)] public string? Notes { get; set; }
}

public class ContactUpsertRequest
{
    [Required, StringLength(200)] public string Name { get; set; } = "";
    [StringLength(100)] public string? Role { get; set; }
    [StringLength(254), EmailAddress] public string? Email { get; set; }
    [StringLength(50)] public string? Phone { get; set; }
    [StringLength(100)] public string? PreferredChannel { get; set; }
    [StringLength(2000)] public string? Notes { get; set; }
}

public class InvoiceUpsertRequest
{
    [Required, Range(0, 1_000_000_000)] public decimal Amount { get; set; }
    [StringLength(10)] public string Currency { get; set; } = "USD";
    [Required] public DateOnly IssuedDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    [StringLength(30)] public string Status { get; set; } = "Draft";
}

public class NoteCreateRequest
{
    [Required, StringLength(10000)] public string Body { get; set; } = "";
}
