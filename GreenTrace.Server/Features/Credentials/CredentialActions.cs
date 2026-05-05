using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Credentials;

[Mutation<CallResult>]
public record CreateCredential(
    long actorId, string type, string? issuer, string? reference,
    DateTimeOffset? issuedAt, DateTimeOffset? expiresAt);

[Mutation<CallResult>]
public record UpdateCredential(
    long id, string? type, string? issuer, string? reference,
    DateTimeOffset? issuedAt, DateTimeOffset? expiresAt, bool? isActive);

[Mutation<CallResult>]
public record DeactivateCredential(long id);
