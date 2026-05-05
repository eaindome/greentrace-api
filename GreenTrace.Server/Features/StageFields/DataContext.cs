using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.StageFields;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<StageField> stageFields { get; set; }

    private void ConfigureStageFields(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StageField>(entity =>
        {
            entity.HasKey(sf => sf.id);

            // Code is unique per stage
            entity.HasIndex(sf => new { sf.stageId, sf.code }).IsUnique();

            entity.Property(sf => sf.code).IsRequired().HasMaxLength(64);
            entity.Property(sf => sf.label).IsRequired().HasMaxLength(256);
            entity.Property(sf => sf.dataType).IsRequired().HasMaxLength(32);

            // JSONB columns
            entity.Property(sf => sf.validation).HasColumnType("jsonb");
            entity.Property(sf => sf.options).HasColumnType("jsonb");
            entity.Property(sf => sf.uiHint).HasColumnType("jsonb");
        });
    }
}
