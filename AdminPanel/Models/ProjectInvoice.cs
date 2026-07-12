namespace AdminPanel.Models;

public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    Overdue
}

public class ProjectInvoice
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateOnly IssuedDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
}
