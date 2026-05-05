using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.ActorRoles;

public class ActorRoleHandler
{
    public static async Task<CallResult> Handle(
        CreateActorRole input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsCreate)) return CallResult.NotPermitted();

        if (!authContext.isPlatformAdmin && input.organizationId != authContext.organizationId)
            return CallResult.Error("You can only create actor roles in your own organization");

        if (input.domainId.HasValue)
        {
            var domain = await db.domains.FindAsync([input.domainId.Value], cancellation);
            if (domain == null)
                return CallResult.Error("Domain not found");
            if (domain.organizationId != input.organizationId)
                return CallResult.Error("Domain does not belong to this organization");
        }

        var codeExists = await db.actorRoles
            .AnyAsync(ar => ar.organizationId == input.organizationId && ar.code == input.code, cancellation);
        if (codeExists)
            return CallResult.Error("An actor role with this code already exists in this organization");

        var actorRole = new ActorRole
        {
            organizationId = input.organizationId,
            domainId = input.domainId,
            code = input.code,
            label = input.label,
            description = input.description,
            allowedStages = input.allowedStages,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.actorRoles.Add(actorRole);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Actor role created");
    }

    public static async Task<CallResult> Handle(
        UpdateActorRole input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsUpdate)) return CallResult.NotPermitted();

        var actorRole = await db.actorRoles.FindAsync([input.id], cancellation);
        if (actorRole == null)
            return CallResult.Error("Actor role not found");

        if (!authContext.isPlatformAdmin && actorRole.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.label != null) actorRole.label = input.label;
        if (input.description != null) actorRole.description = input.description;
        if (input.allowedStages != null) actorRole.allowedStages = input.allowedStages;
        if (input.isActive.HasValue) actorRole.isActive = input.isActive.Value;
        actorRole.updatedBy = authContext.email ?? "system";

        await db.SaveChangesAsync(cancellation);
        return CallResult.Ok("Actor role updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateActorRole input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsDelete)) return CallResult.NotPermitted();

        var actorRole = await db.actorRoles.FindAsync([input.id], cancellation);
        if (actorRole == null)
            return CallResult.Error("Actor role not found");

        if (!authContext.isPlatformAdmin && actorRole.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        actorRole.isActive = false;
        actorRole.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Actor role deactivated");
    }
}
