using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Roles;

[Mutation<CallResult>]
public record CreateRole(string name, string? description, string[] permissions);

[Mutation<CallResult>]
public record UpdateRole(long id, string? description, string[]? permissions, bool? isActive);

[Mutation<CallResult>]
public record DeactivateRole(long id);
