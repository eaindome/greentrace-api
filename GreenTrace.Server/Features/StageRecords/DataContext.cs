using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.StageRecords;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<StageRecord> stageRecords { get; set; }

    private void ConfigureStageRecords(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StageRecord>(entity =>
        {
            entity.HasKey(sr => sr.id);

            entity.HasIndex(sr => new { sr.organizationId, sr.batchId });
            entity.HasIndex(sr => new { sr.organizationId, sr.stageId });
            entity.HasIndex(sr => new { sr.organizationId, sr.actorId });
            entity.HasIndex(sr => new { sr.batchId, sr.stageId });

            entity.Property(sr => sr.status).IsRequired().HasMaxLength(32);
            entity.Property(sr => sr.currency).HasMaxLength(8);
            entity.Property(sr => sr.geoPoint).HasColumnType("jsonb");
            entity.Property(sr => sr.notes).HasMaxLength(2048);
            entity.Property(sr => sr.validationNotes).HasMaxLength(2048);
            entity.Property(sr => sr.quantity).HasPrecision(18, 4);
            entity.Property(sr => sr.unitPrice).HasPrecision(18, 4);

            entity.HasOne(sr => sr.organization)
                .WithMany()
                .HasForeignKey(sr => sr.organizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sr => sr.batch)
                .WithMany(b => b.stageRecords)
                .HasForeignKey(sr => sr.batchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sr => sr.stage)
                .WithMany()
                .HasForeignKey(sr => sr.stageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sr => sr.actor)
                .WithMany()
                .HasForeignKey(sr => sr.actorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(sr => sr.recordedByUser)
                .WithMany()
                .HasForeignKey(sr => sr.recordedBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(sr => sr.validatedByUser)
                .WithMany()
                .HasForeignKey(sr => sr.validatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(sr => !sr.isDeleted);
        });
    }
}
