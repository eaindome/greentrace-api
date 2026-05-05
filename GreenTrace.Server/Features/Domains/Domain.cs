using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;
using GreenTrace.Server.Features.Stages;

namespace GreenTrace.Server.Features.Domains;

/// <summary>
/// A supply chain type (e.g. Cocoa, Gold, Pharma).
/// Each domain defines its own pipeline of stages.
/// </summary>
public record Domain : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public string name { get; set; } = default!;
    public string code { get; set; } = default!;
    public string? description { get; set; }
    public bool isActive { get; set; } = true;

    // Navigation
    public Organizations.Organization? organization { get; set; }
    public ICollection<Stage>? stages { get; set; }
}
