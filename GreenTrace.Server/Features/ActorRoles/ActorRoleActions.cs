using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.ActorRoles;

[Mutation<CallResult>]
public record CreateActorRole(long organizationId, long? domainId, string code, string label, string? description, long[]? allowedStages);

[Mutation<CallResult>]
public record UpdateActorRole(long id, string? label, string? description, long[]? allowedStages, bool? isActive);

[Mutation<CallResult>]
public record DeactivateActorRole(long id);
