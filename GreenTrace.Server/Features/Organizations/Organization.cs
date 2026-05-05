using GreenTrace.Server.Common.Data;

namespace GreenTrace.Server.Features.Organizations;

/// <summary>
/// The tenant root. Every entity in the system belongs to an Organization.
/// </summary>
public record Organization : Auditable
{
    public string name { get; set; } = default!;
    public string slug { get; set; } = default!;
    public bool isActive { get; set; } = true;
}
