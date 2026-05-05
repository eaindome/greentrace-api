using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.Credentials;

/// <summary>
/// A certification, license, or registration held by an actor
/// (e.g. organic_cert, mining_license, fairtrade).
/// </summary>
public record Credential : Auditable, IHasOrganization
{
    public long actorId { get; set; }
    public long organizationId { get; set; }
    public string type { get; set; } = default!;       // e.g. "organic_cert", "mining_license"
    public string? issuer { get; set; }                 // Who issued it
    public string? reference { get; set; }              // Certificate/license number
    public DateTimeOffset? issuedAt { get; set; }
    public DateTimeOffset? expiresAt { get; set; }
    public bool isActive { get; set; } = true;

    // Navigation
    public Actors.Actor? actor { get; set; }
}
