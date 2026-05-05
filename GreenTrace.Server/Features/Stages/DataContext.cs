using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Stages;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<Stage> stages { get; set; }

    private void ConfigureStages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stage>(entity =>
        {
            entity.HasKey(s => s.id);

            // Sequence is unique per domain (enforces pipeline ordering)
            entity.HasIndex(s => new { s.domainId, s.sequence }).IsUnique();
            // Code is unique per domain
            entity.HasIndex(s => new { s.domainId, s.code }).IsUnique();

            entity.Property(s => s.name).IsRequired().HasMaxLength(256);
            entity.Property(s => s.code).IsRequired().HasMaxLength(64);
            entity.Property(s => s.description).HasMaxLength(1024);

            entity.HasMany(s => s.stageFields)
                .WithOne(sf => sf.stage)
                .HasForeignKey(sf => sf.stageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
