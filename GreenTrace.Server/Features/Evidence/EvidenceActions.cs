using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Evidence;

[Mutation<CallResult>]
public record CreateEvidence(
    long? stageRecordId, long? credentialId,
    string storageKey, string? originalFilename, string? mimeType,
    long? sizeBytes, string? type, string? description);

[Mutation<CallResult>]
public record DeleteEvidence(long id);
