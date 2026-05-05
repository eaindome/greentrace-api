using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.Evidence;

/// <summary>
/// A file attached to a stage record or actor credential (photo, document, certificate, etc.).
/// </summary>
public record Evidence : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long? stageRecordId { get; set; }
    public long? credentialId { get; set; }
    public string storageKey { get; set; } = default!;   // S3 key or file path
    public string? originalFilename { get; set; }
    public string? mimeType { get; set; }
    public long? sizeBytes { get; set; }
    public string type { get; set; } = "other";          // photo, document, certificate, lab_result, receipt, other
    public string? description { get; set; }
    public long? uploadedBy { get; set; }

    // Navigation
    public StageRecords.StageRecord? stageRecord { get; set; }
    public Credentials.Credential? credential { get; set; }
    public Users.User? uploadedByUser { get; set; }
}
