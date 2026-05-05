using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.ActorRoles;

/// <summary>
/// A configurable role that actors can hold within a domain (e.g. farmer, aggregator, processor).
/// DomainId null means the role is org-wide (usable across all domains).
/// AllowedStages null means the role can participate in any stage.
/// </summary>
public record ActorRole : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long? domainId { get; set; }
    public string code { get; set; } = default!;
    public string label { get; set; } = default!;
    public string? description { get; set; }
    public long[]? allowedStages { get; set; }  // Stage IDs this role can participate in; null = all
    public bool isActive { get; set; } = true;

    // Navigation
    public Organizations.Organization? organization { get; set; }
    public Domains.Domain? domain { get; set; }
    public ICollection<Actors.Actor>? actors { get; set; }
}
