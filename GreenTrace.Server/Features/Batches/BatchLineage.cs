using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.Batches;

/// <summary>
/// Tracks batch splits and merges. A parent batch can split into children,
/// or multiple parents can merge into a child.
/// </summary>
public record BatchLineage : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long parentBatchId { get; set; }
    public long childBatchId { get; set; }
    public string type { get; set; } = default!;   // "split" or "merge"
    public decimal? quantity { get; set; }          // How much moved from parent to child
    public string? notes { get; set; }

    // Navigation
    public Batch? parentBatch { get; set; }
    public Batch? childBatch { get; set; }
}
