namespace GreenTrace.Server.Features.Audit;

/// <summary>
/// Immutable change log entry. NOT extending Auditable (would be circular).
/// No FK constraints — audit logs outlive the entities they reference.
/// </summary>
public class AuditLog
{
    public long id { get; set; }
    public long? organizationId { get; set; }
    public long? userId { get; set; }
    public string userEmail { get; set; } = "";
    public string entityType { get; set; } = "";
    public long entityId { get; set; }
    public string action { get; set; } = "";            // "created" | "updated" | "deleted"
    public string? changes { get; set; }                 // JSONB diff: { "field": ["old", "new"] }
    public DateTimeOffset occurredAt { get; set; } = DateTimeOffset.UtcNow;
}
