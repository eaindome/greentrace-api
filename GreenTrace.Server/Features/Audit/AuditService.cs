using System.Text.Json;
using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GreenTrace.Server.Features.Audit;

/// <summary>
/// Static helper that builds AuditLog entries from EF Core ChangeTracker.
/// Called by the AuditInterceptor during SaveChanges.
/// </summary>
public static class AuditService
{
    private static readonly HashSet<string> SkippedFields =
        ["createdBy", "createdOn", "updatedBy", "updatedOn", "revision"];

    public static List<AuditLog> BuildChangeLogs(
        DbContext context, long? userId, string userEmail, long? organizationId)
    {
        var logs = new List<AuditLog>();
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity is not AuditLog)   // Never audit the audit log itself
            .ToList();

        foreach (var entry in entries)
        {
            var action = entry.State switch
            {
                EntityState.Added => Constants.AuditCreated,
                EntityState.Modified => Constants.AuditUpdated,
                EntityState.Deleted => Constants.AuditDeleted,
                _ => null
            };
            if (action == null) continue;

            var entityId = GetEntityId(entry);
            var entryOrgId = organizationId;

            // Try to get organizationId from the entity itself
            if (entry.Entity is IHasOrganization orgEntity)
                entryOrgId = orgEntity.organizationId;

            string? changes = null;
            if (entry.State == EntityState.Modified)
                changes = BuildDiff(entry);

            logs.Add(new AuditLog
            {
                organizationId = entryOrgId,
                userId = userId,
                userEmail = userEmail,
                entityType = entry.Metadata.ClrType.Name,
                entityId = entityId,
                action = action,
                changes = changes,
                occurredAt = DateTimeOffset.UtcNow
            });
        }

        return logs;
    }

    private static string? BuildDiff(EntityEntry entry)
    {
        var diff = new Dictionary<string, object?[]>();

        foreach (var prop in entry.Properties)
        {
            if (SkippedFields.Contains(prop.Metadata.Name)) continue;
            if (!prop.IsModified) continue;

            var original = prop.OriginalValue;
            var current = prop.CurrentValue;

            if (Equals(original, current)) continue;

            diff[prop.Metadata.Name] = [original, current];
        }

        return diff.Count > 0
            ? JsonSerializer.Serialize(diff, new JsonSerializerOptions { WriteIndented = false })
            : null;
    }

    private static long GetEntityId(EntityEntry entry)
    {
        if (entry.Entity is IHasId hasId)
            return hasId.id;

        var idProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "id" || p.Metadata.Name == "Id");
        if (idProp?.CurrentValue is long longId)
            return longId;

        return 0;
    }
}
