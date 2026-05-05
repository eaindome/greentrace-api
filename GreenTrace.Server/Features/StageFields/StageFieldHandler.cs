using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.StageFields;

public class StageFieldHandler
{
    public static async Task<CallResult> Handle(
        CreateStageField input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StagesCreate)) return CallResult.NotPermitted();

        var stage = await db.stages
            .Include(s => s.domain)
            .FirstOrDefaultAsync(s => s.id == input.stageId, cancellation);
        if (stage == null)
            return CallResult.Error("Stage not found");

        if (!authContext.isPlatformAdmin && stage.domain?.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        // Validate data type
        var validTypes = new[] {
            Constants.FieldTypeText, Constants.FieldTypeNumber, Constants.FieldTypeDecimal,
            Constants.FieldTypeBoolean, Constants.FieldTypeDate, Constants.FieldTypeSelect,
            Constants.FieldTypeFile, Constants.FieldTypeGeoPoint
        };
        if (!validTypes.Contains(input.dataType))
            return CallResult.Error($"Invalid data type '{input.dataType}'");

        var codeExists = await db.stageFields
            .AnyAsync(sf => sf.stageId == input.stageId && sf.code == input.code, cancellation);
        if (codeExists)
            return CallResult.Error("A field with this code already exists in this stage");

        // Auto-assign fieldOrder if not provided
        var fieldOrder = input.fieldOrder
            ?? (await db.stageFields.Where(sf => sf.stageId == input.stageId).MaxAsync(sf => (int?)sf.fieldOrder, cancellation) ?? 0) + 1;

        var field = new StageField
        {
            stageId = input.stageId,
            code = input.code,
            label = input.label,
            dataType = input.dataType,
            isRequired = input.isRequired,
            fieldOrder = fieldOrder,
            validation = input.validation,
            options = input.options,
            uiHint = input.uiHint,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.stageFields.Add(field);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage field created");
    }

    public static async Task<CallResult> Handle(
        UpdateStageField input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StagesUpdate)) return CallResult.NotPermitted();

        var field = await db.stageFields
            .Include(sf => sf.stage).ThenInclude(s => s!.domain)
            .FirstOrDefaultAsync(sf => sf.id == input.id, cancellation);
        if (field == null)
            return CallResult.Error("Stage field not found");

        if (!authContext.isPlatformAdmin && field.stage?.domain?.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.label != null) field.label = input.label;
        if (input.dataType != null) field.dataType = input.dataType;
        if (input.isRequired.HasValue) field.isRequired = input.isRequired.Value;
        if (input.fieldOrder.HasValue) field.fieldOrder = input.fieldOrder.Value;
        if (input.validation != null) field.validation = input.validation;
        if (input.options != null) field.options = input.options;
        if (input.uiHint != null) field.uiHint = input.uiHint;
        if (input.isActive.HasValue) field.isActive = input.isActive.Value;
        field.updatedBy = authContext.email ?? "system";

        await db.SaveChangesAsync(cancellation);
        return CallResult.Ok("Stage field updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateStageField input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.StagesDelete)) return CallResult.NotPermitted();

        var field = await db.stageFields
            .Include(sf => sf.stage).ThenInclude(s => s!.domain)
            .FirstOrDefaultAsync(sf => sf.id == input.id, cancellation);
        if (field == null)
            return CallResult.Error("Stage field not found");

        if (!authContext.isPlatformAdmin && field.stage?.domain?.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        field.isActive = false;
        field.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Stage field deactivated");
    }
}
