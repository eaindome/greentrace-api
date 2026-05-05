using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Batches;

public class BatchHandler
{
    public static async Task<CallResult> Handle(
        CreateBatch input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.BatchesCreate)) return CallResult.NotPermitted();

        if (!authContext.isPlatformAdmin && input.organizationId != authContext.organizationId)
            return CallResult.Error("You can only create batches in your own organization");

        var domain = await db.domains.FindAsync([input.domainId], cancellation);
        if (domain == null) return CallResult.Error("Domain not found");
        if (domain.organizationId != input.organizationId)
            return CallResult.Error("Domain does not belong to this organization");

        var codeExists = await db.batches
            .AnyAsync(b => b.organizationId == input.organizationId && b.batchCode == input.batchCode, cancellation);
        if (codeExists) return CallResult.Error("A batch with this code already exists");

        var totalValue = (input.initialQuantity ?? 0) * (input.initialUnitPrice ?? 0);

        var batch = new Batch
        {
            organizationId = input.organizationId,
            domainId = input.domainId,
            productId = input.productId,
            batchCode = input.batchCode,
            originActorId = input.originActorId,
            originDate = input.originDate,
            originLocation = input.originLocation,
            initialQuantity = input.initialQuantity,
            initialUnitPrice = input.initialUnitPrice,
            currentQuantity = input.initialQuantity,
            totalValue = totalValue,
            metadata = input.metadata,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.batches.Add(batch);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Batch created");
    }

    public static async Task<CallResult> Handle(
        UpdateBatch input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.BatchesUpdate)) return CallResult.NotPermitted();

        var batch = await db.batches.FindAsync([input.id], cancellation);
        if (batch == null) return CallResult.Error("Batch not found");

        if (!authContext.isPlatformAdmin && batch.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.productId.HasValue) batch.productId = input.productId;
        if (input.originLocation != null) batch.originLocation = input.originLocation;
        if (input.currentQuantity.HasValue) batch.currentQuantity = input.currentQuantity;
        if (input.metadata != null) batch.metadata = input.metadata;

        if (input.status != null)
        {
            var validStatuses = new[] { Constants.BatchOpen, Constants.BatchInProgress, Constants.BatchCompleted, Constants.BatchArchived };
            if (!validStatuses.Contains(input.status))
                return CallResult.Error($"Invalid status '{input.status}'");
            batch.status = input.status;
        }

        batch.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Batch updated");
    }

    public static async Task<CallResult> Handle(
        DeleteBatch input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.BatchesDelete)) return CallResult.NotPermitted();

        var batch = await db.batches.FindAsync([input.id], cancellation);
        if (batch == null) return CallResult.Error("Batch not found");

        if (!authContext.isPlatformAdmin && batch.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        batch.isDeleted = true;
        batch.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Batch deleted");
    }

    public static async Task<CallResult> Handle(
        SplitBatch input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.BatchesCreate)) return CallResult.NotPermitted();

        var parent = await db.batches.FindAsync([input.parentBatchId], cancellation);
        if (parent == null) return CallResult.Error("Parent batch not found");

        if (!authContext.isPlatformAdmin && parent.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (parent.currentQuantity.HasValue && input.quantity > parent.currentQuantity.Value)
            return CallResult.Error("Split quantity exceeds parent's current quantity");

        var codeExists = await db.batches
            .AnyAsync(b => b.organizationId == parent.organizationId && b.batchCode == input.childBatchCode, cancellation);
        if (codeExists) return CallResult.Error("A batch with this code already exists");

        // Create child batch
        var child = new Batch
        {
            organizationId = parent.organizationId,
            domainId = parent.domainId,
            productId = parent.productId,
            batchCode = input.childBatchCode,
            originActorId = parent.originActorId,
            originDate = parent.originDate,
            originLocation = parent.originLocation,
            initialQuantity = input.quantity,
            currentQuantity = input.quantity,
            currentStageId = parent.currentStageId,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.batches.Add(child);

        // Reduce parent quantity
        if (parent.currentQuantity.HasValue)
            parent.currentQuantity -= input.quantity;
        parent.updatedBy = authContext.email ?? "system";

        await db.SaveChangesAsync(cancellation);

        // Record lineage
        var lineage = new BatchLineage
        {
            organizationId = parent.organizationId,
            parentBatchId = parent.id,
            childBatchId = child.id,
            type = "split",
            quantity = input.quantity,
            notes = input.notes,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.batchLineages.Add(lineage);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Batch split successfully");
    }

    public static async Task<CallResult> Handle(
        MergeBatches input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.BatchesCreate)) return CallResult.NotPermitted();

        if (input.parentBatchIds.Length < 2)
            return CallResult.Error("At least 2 batches required for merge");

        var parents = await db.batches
            .Where(b => input.parentBatchIds.Contains(b.id))
            .ToListAsync(cancellation);

        if (parents.Count != input.parentBatchIds.Length)
            return CallResult.Error("One or more parent batches not found");

        var orgId = parents[0].organizationId;
        if (!authContext.isPlatformAdmin && orgId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (parents.Any(p => p.organizationId != orgId))
            return CallResult.Error("All batches must belong to the same organization");

        if (parents.Select(p => p.domainId).Distinct().Count() > 1)
            return CallResult.Error("All batches must belong to the same domain");

        var codeExists = await db.batches
            .AnyAsync(b => b.organizationId == orgId && b.batchCode == input.childBatchCode, cancellation);
        if (codeExists) return CallResult.Error("A batch with this code already exists");

        var totalQuantity = parents.Sum(p => p.currentQuantity ?? 0);

        var child = new Batch
        {
            organizationId = orgId,
            domainId = parents[0].domainId,
            productId = parents[0].productId,
            batchCode = input.childBatchCode,
            initialQuantity = totalQuantity,
            currentQuantity = totalQuantity,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.batches.Add(child);
        await db.SaveChangesAsync(cancellation);

        // Record lineage for each parent
        foreach (var parent in parents)
        {
            var lineage = new BatchLineage
            {
                organizationId = orgId,
                parentBatchId = parent.id,
                childBatchId = child.id,
                type = "merge",
                quantity = parent.currentQuantity,
                notes = input.notes,
                createdBy = authContext.email ?? "system",
                updatedBy = authContext.email ?? "system"
            };
            db.batchLineages.Add(lineage);

            // Mark parents as archived
            parent.status = Constants.BatchArchived;
            parent.updatedBy = authContext.email ?? "system";
        }

        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Batches merged successfully");
    }
}
