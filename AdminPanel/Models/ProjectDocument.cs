namespace AdminPanel.Models;

public enum DocumentType
{
    Contract,
    Nda,
    Proposal,
    Invoice,
    Other
}

public class ProjectDocument
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public DocumentType Type { get; set; } = DocumentType.Other;
    public string Title { get; set; } = "";
    public string StoragePath { get; set; } = "";
    public string MimeType { get; set; } = "";
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
