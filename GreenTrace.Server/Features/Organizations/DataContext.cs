using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Organizations;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

/// <summary>
/// Partial DbContext — adds Organization DbSet.
/// Each feature registers its own entities this way.
/// </summary>
partial class AppDbContext
{
    public DbSet<Organization> organizations { get; set; }

    // Called from OnModelCreating — add org-specific config here
    private void ConfigureOrganizations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(x => x.id);
            entity.HasIndex(x => x.slug).IsUnique();
        });
    }
}
