using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.Batches;

/// <summary>
/// A trackable lot moving through the supply chain pipeline.
/// Status lifecycle: open → in_progress → completed → archived.
/// </summary>
public record Batch : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long domainId { get; set; }
    public long? productId { get; set; }
    public string batchCode { get; set; } = default!;   // Human-readable, e.g. "GH-COC-2024-0042"
    public long? originActorId { get; set; }
    public DateTimeOffset? originDate { get; set; }
    public string? originLocation { get; set; }          // JSONB — GPS + region
    public decimal? initialQuantity { get; set; }
    public decimal? initialUnitPrice { get; set; }
    public decimal? currentQuantity { get; set; }
    public long? currentStageId { get; set; }            // Last completed stage
    public decimal totalValue { get; set; }
    public string status { get; set; } = Common.Constants.BatchOpen;
    public string? metadata { get; set; }                // JSONB — domain-specific batch data
    public bool isDeleted { get; set; }

    // Navigation
    public Organizations.Organization? organization { get; set; }
    public Domains.Domain? domain { get; set; }
    public Products.Product? product { get; set; }
    public Actors.Actor? originActor { get; set; }
    public Stages.Stage? currentStage { get; set; }
    public ICollection<StageRecords.StageRecord>? stageRecords { get; set; }
}
