using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.StageFields;

namespace GreenTrace.Server.Features.Stages;

/// <summary>
/// An ordered step in a domain's pipeline (e.g. Harvest → Aggregation → Processing).
/// Sequence is unique per domain and defines the pipeline order.
/// </summary>
public record Stage : Auditable
{
    public long domainId { get; set; }
    public string name { get; set; } = default!;
    public string code { get; set; } = default!;
    public short sequence { get; set; }
    public bool isRequired { get; set; } = true;
    public string? description { get; set; }
    public bool isActive { get; set; } = true;

    // Navigation
    public Domains.Domain? domain { get; set; }
    public ICollection<StageField>? stageFields { get; set; }
}
