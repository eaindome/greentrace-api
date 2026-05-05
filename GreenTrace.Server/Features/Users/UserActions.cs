using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Users;

[Mutation<CallResult>]
public record CreateUser(string email, string password, string fullName, string? phone, long organizationId, string roleName);

[Mutation<CallResult>]
public record UpdateUser(long id, string? fullName, string? phone, string? roleName, bool? isActive);

[Mutation<CallResult>]
public record DeactivateUser(long id);
