using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Actors;

[Mutation<CallResult>]
public record CreateActor(
    long organizationId, long? roleId, string name, string? externalId,
    string? contact, string? location, string? registrationMeta);

[Mutation<CallResult>]
public record UpdateActor(
    long id, long? roleId, string? name, string? externalId,
    string? contact, string? location, string? registrationMeta, bool? isActive);

[Mutation<CallResult>]
public record DeactivateActor(long id);
