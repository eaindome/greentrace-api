using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Evidence;

public class EvidenceHandler
{
    public static async Task<CallResult> Handle(
        CreateEvidence input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StageRecordsCreate)) return CallResult.NotPermitted();

        if (input.stageRecordId == null && input.credentialId == null)
            return CallResult.Error("Evidence must be linked to a stage record or credential");

        long orgId;

        if (input.stageRecordId.HasValue)
        {
            var record = await db.stageRecords.FindAsync([input.stageRecordId.Value], cancellation);
            if (record == null) return CallResult.Error("Stage record not found");

            if (record.status == Constants.RecordValidated)
                return CallResult.Error("Cannot add evidence to a validated record");

            orgId = record.organizationId;
        }
        else
        {
            var credential = await db.credentials.FindAsync([input.credentialId!.Value], cancellation);
            if (credential == null) return CallResult.Error("Credential not found");
            orgId = credential.organizationId;
        }

        if (!authContext.isPlatformAdmin && orgId != authContext.organizationId)
            return CallResult.NotPermitted();

        var evidence = new Evidence
        {
            organizationId = orgId,
            stageRecordId = input.stageRecordId,
            credentialId = input.credentialId,
            storageKey = input.storageKey,
            originalFilename = input.originalFilename,
            mimeType = input.mimeType,
            sizeBytes = input.sizeBytes,
            type = input.type ?? "other",
            description = input.description,
            uploadedBy = authContext.userId,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.evidences.Add(evidence);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Evidence created");
    }

    public static async Task<CallResult> Handle(
        DeleteEvidence input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StageRecordsUpdate)) return CallResult.NotPermitted();

        var evidence = await db.evidences.FindAsync([input.id], cancellation);
        if (evidence == null) return CallResult.Error("Evidence not found");

        if (!authContext.isPlatformAdmin && evidence.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        // Don't allow deleting evidence on validated records
        if (evidence.stageRecordId.HasValue)
        {
            var record = await db.stageRecords
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(sr => sr.id == evidence.stageRecordId, cancellation);
            if (record?.status == Constants.RecordValidated)
                return CallResult.Error("Cannot delete evidence from a validated record");
        }

        db.evidences.Remove(evidence);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Evidence deleted");
    }
}
