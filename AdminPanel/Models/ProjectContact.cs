namespace AdminPanel.Models;

public class ProjectContact
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public string Name { get; set; } = "";
    public string? Role { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PreferredChannel { get; set; }
    public string? Notes { get; set; }
}
