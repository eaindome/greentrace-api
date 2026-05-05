using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.ValueChain;

/// <summary>
/// Materialized value attribution — one row per validated StageRecord.
/// Captures "who earned what" at each supply chain step.
/// Immutable: created by the system during validation, never mutated.
/// </summary>
public record ValueRecord : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long batchId { get; set; }
    public long stageRecordId { get; set; }
    public long stageId { get; set; }
    public long? actorId { get; set; }
    public decimal quantity { get; set; }
    public decimal unitPrice { get; set; }
    public decimal totalValue { get; set; }             // quantity * unitPrice
    public string currency { get; set; } = "USD";
    public DateTimeOffset recordedAt { get; set; }
    public DateTimeOffset? validatedAt { get; set; }

    // Navigation
    public Organizations.Organization? organization { get; set; }
    public Batches.Batch? batch { get; set; }
    public StageRecords.StageRecord? stageRecord { get; set; }
    public Stages.Stage? stage { get; set; }
    public Actors.Actor? actor { get; set; }
}
