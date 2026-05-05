using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Domains;

[Mutation<CallResult>]
public record CreateDomain(long organizationId, string name, string code, string? description);

[Mutation<CallResult>]
public record UpdateDomain(long id, string? name, string? description, bool? isActive);

[Mutation<CallResult>]
public record DeactivateDomain(long id);
