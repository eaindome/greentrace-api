using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Actors;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<Actor> actors { get; set; }

    private void ConfigureActors(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(a => a.id);

            entity.HasIndex(a => a.organizationId);
            entity.HasIndex(a => new { a.organizationId, a.externalId })
                .IsUnique()
                .HasFilter("\"externalId\" IS NOT NULL");

            entity.Property(a => a.name).IsRequired().HasMaxLength(256);
            entity.Property(a => a.externalId).HasMaxLength(128);

            // JSONB columns
            entity.Property(a => a.contact).HasColumnType("jsonb");
            entity.Property(a => a.location).HasColumnType("jsonb");
            entity.Property(a => a.registrationMeta).HasColumnType("jsonb");

            entity.HasOne(a => a.organization)
                .WithMany()
                .HasForeignKey(a => a.organizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.role)
                .WithMany(ar => ar.actors)
                .HasForeignKey(a => a.roleId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(a => a.registeredByUser)
                .WithMany()
                .HasForeignKey(a => a.registeredBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(a => a.credentials)
                .WithOne(c => c.actor)
                .HasForeignKey(c => c.actorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
