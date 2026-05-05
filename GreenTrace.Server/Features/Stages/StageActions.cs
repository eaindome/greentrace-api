using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Stages;

[Mutation<CallResult>]
public record CreateStage(long domainId, string name, string code, short sequence, bool isRequired, string? description);

[Mutation<CallResult>]
public record UpdateStage(long id, string? name, short? sequence, bool? isRequired, string? description, bool? isActive);

[Mutation<CallResult>]
public record DeactivateStage(long id);
