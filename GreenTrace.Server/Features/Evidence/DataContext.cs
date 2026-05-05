using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Evidence;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<Evidence> evidences { get; set; }

    private void ConfigureEvidence(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Evidence>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.HasIndex(e => new { e.organizationId, e.stageRecordId });
            entity.HasIndex(e => e.credentialId);

            entity.Property(e => e.storageKey).IsRequired().HasMaxLength(1024);
            entity.Property(e => e.originalFilename).HasMaxLength(512);
            entity.Property(e => e.mimeType).HasMaxLength(128);
            entity.Property(e => e.type).IsRequired().HasMaxLength(32);
            entity.Property(e => e.description).HasMaxLength(1024);

            entity.HasOne(e => e.stageRecord)
                .WithMany(sr => sr.evidence)
                .HasForeignKey(e => e.stageRecordId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.credential)
                .WithMany()
                .HasForeignKey(e => e.credentialId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.uploadedByUser)
                .WithMany()
                .HasForeignKey(e => e.uploadedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
