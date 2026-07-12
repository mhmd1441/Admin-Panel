using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Models;

namespace AdminPanel.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectContact> ProjectContacts => Set<ProjectContact>();
    public DbSet<ProjectDocument> ProjectDocuments => Set<ProjectDocument>();
    public DbSet<ProjectInvoice> ProjectInvoices => Set<ProjectInvoice>();
    public DbSet<ProjectNote> ProjectNotes => Set<ProjectNote>();
    public DbSet<ProjectRequirement> ProjectRequirements => Set<ProjectRequirement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(entity =>
        {
            entity.Property(p => p.RateAmount).HasPrecision(18, 2);
            entity.Property(p => p.BudgetTotal).HasPrecision(18, 2);

            entity.HasMany(p => p.Contacts).WithOne(c => c.Project!)
                .HasForeignKey(c => c.ProjectId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(p => p.Documents).WithOne(d => d.Project!)
                .HasForeignKey(d => d.ProjectId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(p => p.Invoices).WithOne(i => i.Project!)
                .HasForeignKey(i => i.ProjectId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(p => p.TimelineNotes).WithOne(n => n.Project!)
                .HasForeignKey(n => n.ProjectId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(p => p.Requirements).WithOne(r => r.Project!)
                .HasForeignKey(r => r.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProjectInvoice>().Property(i => i.Amount).HasPrecision(18, 2);
    }
}
