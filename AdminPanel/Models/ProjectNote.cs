namespace AdminPanel.Models;

public class ProjectNote
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
