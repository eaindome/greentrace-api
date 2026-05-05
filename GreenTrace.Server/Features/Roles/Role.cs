using GreenTrace.Server.Common.Data.Contracts;
using Microsoft.AspNetCore.Identity;

namespace GreenTrace.Server.Features.Roles;

/// <summary>
/// Application role with string-array permissions.
/// Extends IdentityRole for ASP.NET Identity integration.
/// </summary>
public class Role : IdentityRole<long>, IAuditable
{
    // Bridge Identity's Id with our IAuditable.id
    long IHasId.id { get => Id; set => Id = value; }

    public string[] permissions { get; set; } = [];
    public string? description { get; set; }
    public bool isActive { get; set; } = true;

    // IAuditable
    public string createdBy { get; set; } = "";
    public DateTimeOffset createdOn { get; set; } = DateTimeOffset.UtcNow;
    public string updatedBy { get; set; } = "";
    public DateTimeOffset? updatedOn { get; set; } = DateTimeOffset.UtcNow;
    public int revision { get; set; }
}
