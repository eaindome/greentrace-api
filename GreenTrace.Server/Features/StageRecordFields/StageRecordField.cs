using GreenTrace.Server.Common.Data;

namespace GreenTrace.Server.Features.StageRecordFields;

/// <summary>
/// A captured value for a specific StageField on a StageRecord.
/// Value is stored as string — typed by the referenced StageField.DataType.
/// </summary>
public record StageRecordField : Auditable
{
    public long stageRecordId { get; set; }
    public long stageFieldId { get; set; }
    public string? value { get; set; }

    // Navigation
    public StageRecords.StageRecord? stageRecord { get; set; }
    public StageFields.StageField? stageField { get; set; }
}
