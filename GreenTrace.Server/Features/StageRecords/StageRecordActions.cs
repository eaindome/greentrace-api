using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.StageRecords;

/// <summary>
/// A field value to capture alongside a stage record.
/// </summary>
public record StageRecordFieldInput(long stageFieldId, string? value);

[Mutation<CallResult>]
public record CreateStageRecord(
    long batchId, long stageId, long? actorId,
    decimal? quantity, decimal? unitPrice, string? currency,
    string? geoPoint, string? notes, DateTimeOffset? recordedAt,
    StageRecordFieldInput[]? fields);

[Mutation<CallResult>]
public record SubmitStageRecord(long id);

[Mutation<CallResult>]
public record ValidateStageRecord(long id, string? validationNotes);

[Mutation<CallResult>]
public record RejectStageRecord(long id, string? validationNotes);

[Mutation<CallResult>]
public record DeleteStageRecord(long id);
