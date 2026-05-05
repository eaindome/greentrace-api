using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Stages;

public class StageHandler
{
    public static async Task<CallResult> Handle(
        CreateStage input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StagesCreate)) return CallResult.NotPermitted();

        var domain = await db.domains.FindAsync([input.domainId], cancellation);
        if (domain == null)
            return CallResult.Error("Domain not found");

        if (!authContext.isPlatformAdmin && domain.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        // Check unique code within domain
        var codeExists = await db.stages
            .AnyAsync(s => s.domainId == input.domainId && s.code == input.code, cancellation);
        if (codeExists)
            return CallResult.Error("A stage with this code already exists in this domain");

        // Check unique sequence within domain
        var seqExists = await db.stages
            .AnyAsync(s => s.domainId == input.domainId && s.sequence == input.sequence, cancellation);
        if (seqExists)
            return CallResult.Error($"Sequence {input.sequence} is already taken in this domain");

        var stage = new Stage
        {
            domainId = input.domainId,
            name = input.name,
            code = input.code,
            sequence = input.sequence,
            isRequired = input.isRequired,
            description = input.description,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.stages.Add(stage);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage created");
    }

    public static async Task<CallResult> Handle(
        UpdateStage input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StagesUpdate)) return CallResult.NotPermitted();

        var stage = await db.stages.Include(s => s.domain).FirstOrDefaultAsync(s => s.id == input.id, cancellation);
        if (stage == null)
            return CallResult.Error("Stage not found");

        if (!authContext.isPlatformAdmin && stage.domain?.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.name != null) stage.name = input.name;
        if (input.description != null) stage.description = input.description;
        if (input.isRequired.HasValue) stage.isRequired = input.isRequired.Value;
        if (input.isActive.HasValue) stage.isActive = input.isActive.Value;

        if (input.sequence.HasValue && input.sequence.Value != stage.sequence)
        {
            var seqExists = await db.stages
                .AnyAsync(s => s.domainId == stage.domainId && s.sequence == input.sequence.Value && s.id != input.id, cancellation);
            if (seqExists)
                return CallResult.Error($"Sequence {input.sequence.Value} is already taken in this domain");
            stage.sequence = input.sequence.Value;
        }

        stage.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateStage input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StagesDelete)) return CallResult.NotPermitted();

        var stage = await db.stages.Include(s => s.domain).FirstOrDefaultAsync(s => s.id == input.id, cancellation);
        if (stage == null)
            return CallResult.Error("Stage not found");

        if (!authContext.isPlatformAdmin && stage.domain?.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        stage.isActive = false;
        stage.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage deactivated");
    }
}
