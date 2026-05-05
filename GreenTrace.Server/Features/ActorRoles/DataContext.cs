using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.ActorRoles;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<ActorRole> actorRoles { get; set; }

    private void ConfigureActorRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActorRole>(entity =>
        {
            entity.HasKey(ar => ar.id);

            // Code unique per organization
            entity.HasIndex(ar => new { ar.organizationId, ar.code }).IsUnique();

            entity.Property(ar => ar.code).IsRequired().HasMaxLength(64);
            entity.Property(ar => ar.label).IsRequired().HasMaxLength(256);
            entity.Property(ar => ar.description).HasMaxLength(1024);
            entity.Property(ar => ar.allowedStages).HasColumnType("bigint[]");

            entity.HasOne(ar => ar.organization)
                .WithMany()
                .HasForeignKey(ar => ar.organizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ar => ar.domain)
                .WithMany()
                .HasForeignKey(ar => ar.domainId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
