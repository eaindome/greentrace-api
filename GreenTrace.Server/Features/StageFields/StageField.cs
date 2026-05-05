using GreenTrace.Server.Common.Data;

namespace GreenTrace.Server.Features.StageFields;

/// <summary>
/// A data field definition for a stage (e.g. "Weight (kg)", "Moisture %", "GPS Location").
/// Defines the schema that StageRecordFields must conform to.
/// </summary>
public record StageField : Auditable
{
    public long stageId { get; set; }
    public string code { get; set; } = default!;
    public string label { get; set; } = default!;
    public string dataType { get; set; } = Common.Constants.FieldTypeText;
    public bool isRequired { get; set; }
    public int fieldOrder { get; set; } = 1;
    public string? validation { get; set; }  // JSONB — e.g. { "min": 0, "max": 100 }
    public string? options { get; set; }     // JSONB — e.g. ["A","B","C"] for select type
    public string? uiHint { get; set; }      // JSONB — frontend rendering hints
    public bool isActive { get; set; } = true;

    // Navigation
    public Stages.Stage? stage { get; set; }
}
