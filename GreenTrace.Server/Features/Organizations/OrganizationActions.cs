using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Organizations;

[Mutation<CallResult>]
public record CreateOrganization(string name, string slug);

[Mutation<CallResult>]
public record UpdateOrganization(long id, string? name, string? slug);

[Mutation<CallResult>]
public record DeactivateOrganization(long id);
