using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.StageFields;

[Mutation<CallResult>]
public record CreateStageField(
    long stageId, string code, string label, string dataType,
    bool isRequired, int? fieldOrder, string? validation, string? options, string? uiHint);

[Mutation<CallResult>]
public record UpdateStageField(
    long id, string? label, string? dataType, bool? isRequired,
    int? fieldOrder, string? validation, string? options, string? uiHint, bool? isActive);

[Mutation<CallResult>]
public record DeactivateStageField(long id);
