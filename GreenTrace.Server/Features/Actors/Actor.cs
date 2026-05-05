using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.Actors;

/// <summary>
/// A real-world supply chain participant (farmer, miner, dealer, etc.).
/// These are NOT platform users — they are the people/entities being tracked in the chain.
/// </summary>
public record Actor : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long? roleId { get; set; }
    public string name { get; set; } = default!;
    public string? externalId { get; set; }          // Org's own ID for this actor
    public string? contact { get; set; }             // JSONB — phone, email, address
    public string? location { get; set; }            // JSONB — gps coords, region, district, community
    public string? registrationMeta { get; set; }    // JSONB — ID number, cooperative membership, etc.
    public bool isActive { get; set; } = true;
    public long? registeredBy { get; set; }          // FK → User who registered this actor

    // Navigation
    public Organizations.Organization? organization { get; set; }
    public ActorRoles.ActorRole? role { get; set; }
    public Users.User? registeredByUser { get; set; }
    public ICollection<Credentials.Credential>? credentials { get; set; }
}
