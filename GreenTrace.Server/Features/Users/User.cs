using GreenTrace.Server.Common.Data.Contracts;
using GreenTrace.Server.Features.Organizations;
using Microsoft.AspNetCore.Identity;

namespace GreenTrace.Server.Features.Users;

/// <summary>
/// Application user. Extends IdentityUser for ASP.NET Identity integration.
/// Role assignment is via Identity's AspNetUserRoles join table.
/// </summary>
public class User : IdentityUser<long>, IAuditable, IHasOrganization
{
    // Bridge Identity's Id with our IAuditable.id
    long IHasId.id { get => Id; set => Id = value; }

    public string fullName { get; set; } = default!;
    public long organizationId { get; set; }
    public bool isActive { get; set; } = true;

    // Navigation
    public Organization? organization { get; set; }

    // IAuditable
    public string createdBy { get; set; } = "";
    public DateTimeOffset createdOn { get; set; } = DateTimeOffset.UtcNow;
    public string updatedBy { get; set; } = "";
    public DateTimeOffset? updatedOn { get; set; } = DateTimeOffset.UtcNow;
    public int revision { get; set; }
}
