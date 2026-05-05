using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.StageRecordFields;
using GreenTrace.Server.Features.ValueChain;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.StageRecords;

public class StageRecordHandler
{
    public static async Task<CallResult> Handle(
        CreateStageRecord input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StageRecordsCreate)) return CallResult.NotPermitted();

        var batch = await db.batches.FindAsync([input.batchId], cancellation);
        if (batch == null) return CallResult.Error("Batch not found");

        if (!authContext.isPlatformAdmin && batch.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        var stage = await db.stages
            .Include(s => s.domain)
            .FirstOrDefaultAsync(s => s.id == input.stageId, cancellation);
        if (stage == null) return CallResult.Error("Stage not found");
        if (stage.domainId != batch.domainId)
            return CallResult.Error("Stage does not belong to this batch's domain");

        // Enforce stage ordering: check all required previous stages are completed
        var requiredPreviousStages = await db.stages
            .Where(s => s.domainId == batch.domainId && s.sequence < stage.sequence && s.isRequired && s.isActive)
            .Select(s => s.id)
            .ToListAsync(cancellation);

        if (requiredPreviousStages.Count > 0)
        {
            var completedStageIds = await db.stageRecords
                .Where(sr => sr.batchId == batch.id && sr.status == Constants.RecordValidated)
                .Select(sr => sr.stageId)
                .Distinct()
                .ToListAsync(cancellation);

            var missingStages = requiredPreviousStages.Except(completedStageIds).ToList();
            if (missingStages.Count > 0)
                return CallResult.Error("Batch has not completed all required previous stages");
        }

        var record = new StageRecord
        {
            organizationId = batch.organizationId,
            batchId = input.batchId,
            stageId = input.stageId,
            actorId = input.actorId,
            quantity = input.quantity,
            unitPrice = input.unitPrice,
            currency = input.currency ?? "USD",
            geoPoint = input.geoPoint,
            notes = input.notes,
            recordedBy = authContext.userId,
            recordedAt = input.recordedAt ?? DateTimeOffset.UtcNow,
            status = Constants.RecordDraft,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.stageRecords.Add(record);
        await db.SaveChangesAsync(cancellation);

        // Save stage record fields
        if (input.fields is { Length: > 0 })
        {
            foreach (var fieldInput in input.fields)
            {
                var stageField = await db.stageFields.FindAsync([fieldInput.stageFieldId], cancellation);
                if (stageField == null || stageField.stageId != input.stageId) continue;

                var recordField = new StageRecordField
                {
                    stageRecordId = record.id,
                    stageFieldId = fieldInput.stageFieldId,
                    value = fieldInput.value,
                    createdBy = authContext.email ?? "system",
                    updatedBy = authContext.email ?? "system"
                };
                db.stageRecordFields.Add(recordField);
            }

            await db.SaveChangesAsync(cancellation);
        }

        // Update batch current stage and quantity
        batch.currentStageId = input.stageId;
        if (input.quantity.HasValue) batch.currentQuantity = input.quantity;
        if (batch.status == Constants.BatchOpen) batch.status = Constants.BatchInProgress;
        batch.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage record created");
    }

    public static async Task<CallResult> Handle(
        SubmitStageRecord input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StageRecordsUpdate)) return CallResult.NotPermitted();

        var record = await db.stageRecords.FindAsync([input.id], cancellation);
        if (record == null) return CallResult.Error("Stage record not found");

        if (!authContext.isPlatformAdmin && record.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (record.status != Constants.RecordDraft)
            return CallResult.Error($"Can only submit records in '{Constants.RecordDraft}' status");

        // Validate required fields are present
        var requiredFields = await db.stageFields
            .Where(sf => sf.stageId == record.stageId && sf.isRequired && sf.isActive)
            .Select(sf => sf.id)
            .ToListAsync(cancellation);

        if (requiredFields.Count > 0)
        {
            var capturedFieldIds = await db.stageRecordFields
                .Where(srf => srf.stageRecordId == record.id && srf.value != null)
                .Select(srf => srf.stageFieldId)
                .ToListAsync(cancellation);

            var missing = requiredFields.Except(capturedFieldIds).ToList();
            if (missing.Count > 0)
                return CallResult.Error($"Missing {missing.Count} required field(s)");
        }

        record.status = Constants.RecordSubmitted;
        record.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage record submitted for validation");
    }

    public static async Task<CallResult> Handle(
        ValidateStageRecord input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StageRecordsValidate)) return CallResult.NotPermitted();

        var record = await db.stageRecords.FindAsync([input.id], cancellation);
        if (record == null) return CallResult.Error("Stage record not found");

        if (!authContext.isPlatformAdmin && record.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (record.status != Constants.RecordSubmitted)
            return CallResult.Error($"Can only validate records in '{Constants.RecordSubmitted}' status");

        record.status = Constants.RecordValidated;
        record.validatedBy = authContext.userId;
        record.validatedAt = DateTimeOffset.UtcNow;
        record.validationNotes = input.validationNotes;
        record.updatedBy = authContext.email ?? "system";

        // Create ValueRecord (idempotency guard — skip if already exists)
        var alreadyHasValue = await db.valueRecords.AnyAsync(v => v.stageRecordId == record.id, cancellation);
        if (!alreadyHasValue)
        {
            var total = (record.quantity ?? 0) * (record.unitPrice ?? 0);

            var valueRecord = new ValueRecord
            {
                organizationId = record.organizationId,
                batchId = record.batchId,
                stageRecordId = record.id,
                stageId = record.stageId,
                actorId = record.actorId,
                quantity = record.quantity ?? 0,
                unitPrice = record.unitPrice ?? 0,
                totalValue = total,
                currency = record.currency,
                recordedAt = record.recordedAt,
                validatedAt = record.validatedAt,
                createdBy = authContext.email ?? "system",
                updatedBy = authContext.email ?? "system"
            };
            db.valueRecords.Add(valueRecord);

            // Increment batch total value
            var batch = await db.batches.FindAsync([record.batchId], cancellation);
            if (batch != null)
            {
                batch.totalValue += total;
                batch.updatedBy = authContext.email ?? "system";
            }
        }

        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage record validated");
    }

    public static async Task<CallResult> Handle(
        RejectStageRecord input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StageRecordsValidate)) return CallResult.NotPermitted();

        var record = await db.stageRecords.FindAsync([input.id], cancellation);
        if (record == null) return CallResult.Error("Stage record not found");

        if (!authContext.isPlatformAdmin && record.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (record.status != Constants.RecordSubmitted)
            return CallResult.Error($"Can only reject records in '{Constants.RecordSubmitted}' status");

        record.status = Constants.RecordRejected;
        record.validatedBy = authContext.userId;
        record.validatedAt = DateTimeOffset.UtcNow;
        record.validationNotes = input.validationNotes;
        record.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage record rejected");
    }

    public static async Task<CallResult> Handle(
        DeleteStageRecord input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StageRecordsUpdate)) return CallResult.NotPermitted();

        var record = await db.stageRecords.FindAsync([input.id], cancellation);
        if (record == null) return CallResult.Error("Stage record not found");

        if (!authContext.isPlatformAdmin && record.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (record.status == Constants.RecordValidated)
            return CallResult.Error("Validated records cannot be deleted");

        record.isDeleted = true;
        record.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage record deleted");
    }
}
