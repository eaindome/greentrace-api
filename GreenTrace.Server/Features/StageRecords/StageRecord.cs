using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.StageRecords;

/// <summary>
/// A batch at a stage — the core traceability event.
/// Status lifecycle: draft → submitted → validated/rejected.
/// Validated records become immutable.
/// </summary>
public record StageRecord : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long batchId { get; set; }
    public long stageId { get; set; }
    public long? actorId { get; set; }
    public decimal? quantity { get; set; }
    public decimal? unitPrice { get; set; }
    public string currency { get; set; } = "USD";
    public string? geoPoint { get; set; }                // JSONB — {lat, lng, accuracy}
    public string? notes { get; set; }
    public long? recordedBy { get; set; }                // FK → User who entered this
    public DateTimeOffset recordedAt { get; set; } = DateTimeOffset.UtcNow;
    public string status { get; set; } = Common.Constants.RecordDraft;
    public long? validatedBy { get; set; }
    public DateTimeOffset? validatedAt { get; set; }
    public string? validationNotes { get; set; }
    public bool isDeleted { get; set; }

    // Navigation
    public Organizations.Organization? organization { get; set; }
    public Batches.Batch? batch { get; set; }
    public Stages.Stage? stage { get; set; }
    public Actors.Actor? actor { get; set; }
    public Users.User? recordedByUser { get; set; }
    public Users.User? validatedByUser { get; set; }
    public ICollection<StageRecordFields.StageRecordField>? fields { get; set; }
    public ICollection<Evidence.Evidence>? evidence { get; set; }
}
