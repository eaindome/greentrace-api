using GreenTrace.Server.Features.Batches;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<Batch> batches { get; set; }
    public DbSet<BatchLineage> batchLineages { get; set; }

    private void ConfigureBatches(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(b => b.id);

            entity.HasIndex(b => new { b.organizationId, b.batchCode }).IsUnique();
            entity.HasIndex(b => new { b.organizationId, b.status });
            entity.HasIndex(b => new { b.organizationId, b.domainId, b.status });

            entity.Property(b => b.batchCode).IsRequired().HasMaxLength(64);
            entity.Property(b => b.status).IsRequired().HasMaxLength(32);
            entity.Property(b => b.originLocation).HasColumnType("jsonb");
            entity.Property(b => b.metadata).HasColumnType("jsonb");
            entity.Property(b => b.initialQuantity).HasPrecision(18, 4);
            entity.Property(b => b.initialUnitPrice).HasPrecision(18, 4);
            entity.Property(b => b.currentQuantity).HasPrecision(18, 4);
            entity.Property(b => b.totalValue).HasPrecision(18, 4);

            entity.HasOne(b => b.organization)
                .WithMany()
                .HasForeignKey(b => b.organizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.domain)
                .WithMany()
                .HasForeignKey(b => b.domainId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.product)
                .WithMany(p => p.batches)
                .HasForeignKey(b => b.productId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(b => b.originActor)
                .WithMany()
                .HasForeignKey(b => b.originActorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(b => b.currentStage)
                .WithMany()
                .HasForeignKey(b => b.currentStageId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(b => !b.isDeleted);
        });

        modelBuilder.Entity<BatchLineage>(entity =>
        {
            entity.HasKey(bl => bl.id);

            entity.HasIndex(bl => bl.parentBatchId);
            entity.HasIndex(bl => bl.childBatchId);

            entity.Property(bl => bl.type).IsRequired().HasMaxLength(16);
            entity.Property(bl => bl.quantity).HasPrecision(18, 4);
            entity.Property(bl => bl.notes).HasMaxLength(1024);

            entity.HasOne(bl => bl.parentBatch)
                .WithMany()
                .HasForeignKey(bl => bl.parentBatchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(bl => bl.childBatch)
                .WithMany()
                .HasForeignKey(bl => bl.childBatchId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
