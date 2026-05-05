using GreenTrace.Server.Features.ValueChain;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<ValueRecord> valueRecords { get; set; }

    private void ConfigureValueChain(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ValueRecord>(entity =>
        {
            entity.HasKey(v => v.id);

            // One ValueRecord per validated StageRecord
            entity.HasIndex(v => v.stageRecordId).IsUnique();

            // Query indexes
            entity.HasIndex(v => new { v.organizationId, v.batchId });
            entity.HasIndex(v => new { v.organizationId, v.actorId });
            entity.HasIndex(v => new { v.organizationId, v.stageId });

            entity.Property(v => v.quantity).HasPrecision(18, 4);
            entity.Property(v => v.unitPrice).HasPrecision(18, 4);
            entity.Property(v => v.totalValue).HasPrecision(18, 4);
            entity.Property(v => v.currency).HasMaxLength(8);

            entity.HasOne(v => v.organization)
                .WithMany()
                .HasForeignKey(v => v.organizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.batch)
                .WithMany()
                .HasForeignKey(v => v.batchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.stageRecord)
                .WithMany()
                .HasForeignKey(v => v.stageRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.stage)
                .WithMany()
                .HasForeignKey(v => v.stageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.actor)
                .WithMany()
                .HasForeignKey(v => v.actorId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
