using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Actors;

public class ActorHandler
{
    public static async Task<CallResult> Handle(
        CreateActor input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsCreate)) return CallResult.NotPermitted();

        if (!authContext.isPlatformAdmin && input.organizationId != authContext.organizationId)
            return CallResult.Error("You can only create actors in your own organization");

        if (input.roleId.HasValue)
        {
            var role = await db.actorRoles.FindAsync([input.roleId.Value], cancellation);
            if (role == null)
                return CallResult.Error("Actor role not found");
            if (role.organizationId != input.organizationId)
                return CallResult.Error("Actor role does not belong to this organization");
        }

        // Check unique externalId within org
        if (input.externalId != null)
        {
            var extIdExists = await db.actors
                .AnyAsync(a => a.organizationId == input.organizationId && a.externalId == input.externalId, cancellation);
            if (extIdExists)
                return CallResult.Error("An actor with this external ID already exists in this organization");
        }

        var actor = new Actor
        {
            organizationId = input.organizationId,
            roleId = input.roleId,
            name = input.name,
            externalId = input.externalId,
            contact = input.contact,
            location = input.location,
            registrationMeta = input.registrationMeta,
            registeredBy = authContext.userId,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.actors.Add(actor);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Actor created");
    }

    public static async Task<CallResult> Handle(
        UpdateActor input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsUpdate)) return CallResult.NotPermitted();

        var actor = await db.actors.FindAsync([input.id], cancellation);
        if (actor == null)
            return CallResult.Error("Actor not found");

        if (!authContext.isPlatformAdmin && actor.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.roleId.HasValue)
        {
            var role = await db.actorRoles.FindAsync([input.roleId.Value], cancellation);
            if (role == null)
                return CallResult.Error("Actor role not found");
            if (role.organizationId != actor.organizationId)
                return CallResult.Error("Actor role does not belong to this organization");
            actor.roleId = input.roleId;
        }

        if (input.name != null) actor.name = input.name;
        if (input.contact != null) actor.contact = input.contact;
        if (input.location != null) actor.location = input.location;
        if (input.registrationMeta != null) actor.registrationMeta = input.registrationMeta;
        if (input.isActive.HasValue) actor.isActive = input.isActive.Value;

        if (input.externalId != null && input.externalId != actor.externalId)
        {
            var extIdExists = await db.actors
                .AnyAsync(a => a.organizationId == actor.organizationId && a.externalId == input.externalId && a.id != input.id, cancellation);
            if (extIdExists)
                return CallResult.Error("An actor with this external ID already exists in this organization");
            actor.externalId = input.externalId;
        }

        actor.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Actor updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateActor input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsDelete)) return CallResult.NotPermitted();

        var actor = await db.actors.FindAsync([input.id], cancellation);
        if (actor == null)
            return CallResult.Error("Actor not found");

        if (!authContext.isPlatformAdmin && actor.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        actor.isActive = false;
        actor.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Actor deactivated");
    }
}
