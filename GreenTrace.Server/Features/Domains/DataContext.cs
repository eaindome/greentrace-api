using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Domains;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<Domain> domains { get; set; }

    private void ConfigureDomains(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain>(entity =>
        {
            entity.HasKey(d => d.id);

            // Code is unique per organization
            entity.HasIndex(d => new { d.organizationId, d.code }).IsUnique();

            entity.Property(d => d.name).IsRequired().HasMaxLength(256);
            entity.Property(d => d.code).IsRequired().HasMaxLength(64);
            entity.Property(d => d.description).HasMaxLength(1024);

            entity.HasOne(d => d.organization)
                .WithMany()
                .HasForeignKey(d => d.organizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.stages)
                .WithOne(s => s.domain)
                .HasForeignKey(s => s.domainId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
