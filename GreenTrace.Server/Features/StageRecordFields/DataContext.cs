using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.StageRecordFields;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<StageRecordField> stageRecordFields { get; set; }

    private void ConfigureStageRecordFields(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StageRecordField>(entity =>
        {
            entity.HasKey(srf => srf.id);

            // Each field captured once per record
            entity.HasIndex(srf => new { srf.stageRecordId, srf.stageFieldId }).IsUnique();

            entity.Property(srf => srf.value).HasMaxLength(4096);

            entity.HasOne(srf => srf.stageRecord)
                .WithMany(sr => sr.fields)
                .HasForeignKey(srf => srf.stageRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(srf => srf.stageField)
                .WithMany()
                .HasForeignKey(srf => srf.stageFieldId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
