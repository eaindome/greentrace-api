using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Credentials;

public class CredentialHandler
{
    public static async Task<CallResult> Handle(
        CreateCredential input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsCreate)) return CallResult.NotPermitted();

        var actor = await db.actors.FindAsync([input.actorId], cancellation);
        if (actor == null)
            return CallResult.Error("Actor not found");

        if (!authContext.isPlatformAdmin && actor.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        var credential = new Credential
        {
            actorId = input.actorId,
            organizationId = actor.organizationId,
            type = input.type,
            issuer = input.issuer,
            reference = input.reference,
            issuedAt = input.issuedAt,
            expiresAt = input.expiresAt,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.credentials.Add(credential);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Credential created");
    }

    public static async Task<CallResult> Handle(
        UpdateCredential input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsUpdate)) return CallResult.NotPermitted();

        var credential = await db.credentials.FindAsync([input.id], cancellation);
        if (credential == null)
            return CallResult.Error("Credential not found");

        if (!authContext.isPlatformAdmin && credential.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.type != null) credential.type = input.type;
        if (input.issuer != null) credential.issuer = input.issuer;
        if (input.reference != null) credential.reference = input.reference;
        if (input.issuedAt.HasValue) credential.issuedAt = input.issuedAt;
        if (input.expiresAt.HasValue) credential.expiresAt = input.expiresAt;
        if (input.isActive.HasValue) credential.isActive = input.isActive.Value;
        credential.updatedBy = authContext.email ?? "system";

        await db.SaveChangesAsync(cancellation);
        return CallResult.Ok("Credential updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateCredential input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.DomainsDelete)) return CallResult.NotPermitted();

        var credential = await db.credentials.FindAsync([input.id], cancellation);
        if (credential == null)
            return CallResult.Error("Credential not found");

        if (!authContext.isPlatformAdmin && credential.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        credential.isActive = false;
        credential.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Credential deactivated");
    }
}
