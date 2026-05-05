using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Organizations;

public class OrganizationHandler
{
    public static async Task<CallResult> Handle(
        CreateOrganization input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();

        // Check slug uniqueness
        var exists = await db.organizations
            .AnyAsync(o => o.slug == input.slug, cancellation);

        if (exists)
            return CallResult.Error("An organization with this slug already exists");

        var org = new Organization
        {
            name = input.name,
            slug = input.slug,
            createdBy = authContext.username ?? "system",
            updatedBy = authContext.username ?? "system"
        };

        db.Add(org);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Organization created successfully");
    }

    public static async Task<CallResult> Handle(
        UpdateOrganization input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();

        var org = await db.organizations.FindAsync([input.id], cancellation);
        if (org == null)
            return CallResult.Error("Organization not found");

        if (input.name != null) org.name = input.name;
        if (input.slug != null)
        {
            var slugTaken = await db.organizations
                .AnyAsync(o => o.slug == input.slug && o.id != input.id, cancellation);
            if (slugTaken)
                return CallResult.Error("This slug is already taken");
            org.slug = input.slug;
        }

        org.updatedBy = authContext.username ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Organization updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateOrganization input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();

        var org = await db.organizations.FindAsync([input.id], cancellation);
        if (org == null)
            return CallResult.Error("Organization not found");

        org.isActive = false;
        org.updatedBy = authContext.username ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Organization deactivated");
    }
}
