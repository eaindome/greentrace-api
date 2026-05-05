using GreenTrace.Server.Features.Audit;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<AuditLog> auditLogs { get; set; }

    private void ConfigureAudit(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.id);

            entity.HasIndex(a => new { a.entityType, a.entityId });
            entity.HasIndex(a => a.organizationId);
            entity.HasIndex(a => a.occurredAt);
            entity.HasIndex(a => a.userId);

            entity.Property(a => a.entityType).IsRequired().HasMaxLength(128);
            entity.Property(a => a.action).IsRequired().HasMaxLength(32);
            entity.Property(a => a.userEmail).HasMaxLength(256);
            entity.Property(a => a.changes).HasColumnType("jsonb");

            // No FKs — audit logs are independent of entity lifecycle
        });
    }
}
