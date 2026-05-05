using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Credentials;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<Credential> credentials { get; set; }

    private void ConfigureCredentials(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.HasKey(c => c.id);

            entity.HasIndex(c => c.actorId);
            entity.HasIndex(c => c.organizationId);

            entity.Property(c => c.type).IsRequired().HasMaxLength(64);
            entity.Property(c => c.issuer).HasMaxLength(256);
            entity.Property(c => c.reference).HasMaxLength(256);
        });
    }
}
