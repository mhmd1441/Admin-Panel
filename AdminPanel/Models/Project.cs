namespace AdminPanel.Models;

public enum ProjectStatus
{
    Lead,
    ProposalSent,
    Active,
    Paused,
    Completed,
    Cancelled
}

public enum RateType
{
    Hourly,
    Fixed,
    Retainer
}

public class Project
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = "";
    public string ProjectTitle { get; set; } = "";
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Lead;
    public string? Source { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Currency { get; set; } = "USD";
    public RateType RateType { get; set; } = RateType.Hourly;
    public decimal? RateAmount { get; set; }
    public decimal? BudgetTotal { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Priority { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ProjectContact> Contacts { get; set; } = [];
    public List<ProjectDocument> Documents { get; set; } = [];
    public List<ProjectInvoice> Invoices { get; set; } = [];
    public List<ProjectNote> TimelineNotes { get; set; } = [];
    public List<ProjectRequirement> Requirements { get; set; } = [];
}
