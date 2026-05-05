using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Domains;

public class DomainHandler
{
    public static async Task<CallResult> Handle(
        CreateDomain input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsCreate)) return CallResult.NotPermitted();

        if (!authContext.isPlatformAdmin && input.organizationId != authContext.organizationId)
            return CallResult.Error("You can only create domains in your own organization");

        var codeExists = await db.domains
            .AnyAsync(d => d.organizationId == input.organizationId && d.code == input.code, cancellation);
        if (codeExists)
            return CallResult.Error("A domain with this code already exists in this organization");

        var domain = new Domain
        {
            organizationId = input.organizationId,
            name = input.name,
            code = input.code,
            description = input.description,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.domains.Add(domain);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Domain created");
    }

    public static async Task<CallResult> Handle(
        UpdateDomain input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsUpdate)) return CallResult.NotPermitted();

        var domain = await db.domains.FindAsync([input.id], cancellation);
        if (domain == null)
            return CallResult.Error("Domain not found");

        if (!authContext.isPlatformAdmin && domain.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.name != null) domain.name = input.name;
        if (input.description != null) domain.description = input.description;
        if (input.isActive.HasValue) domain.isActive = input.isActive.Value;
        domain.updatedBy = authContext.email ?? "system";

        await db.SaveChangesAsync(cancellation);
        return CallResult.Ok("Domain updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateDomain input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsDelete)) return CallResult.NotPermitted();

        var domain = await db.domains.FindAsync([input.id], cancellation);
        if (domain == null)
            return CallResult.Error("Domain not found");

        if (!authContext.isPlatformAdmin && domain.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        domain.isActive = false;
        domain.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Domain deactivated");
    }
}
