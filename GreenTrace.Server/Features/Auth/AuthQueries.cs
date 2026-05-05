using Cai;
using GreenTrace.Server.Common.Auth;

namespace GreenTrace.Server.Features.Auth;

public record MeResponse(
    long userId,
    long organizationId,
    string? username,
    string? email,
    string? fullName,
    string? role,
    string[] permissions
);

[Queries]
public partial class AuthQueries
{
    public MeResponse? me(AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return null;

        return new MeResponse(
            authContext.userId,
            authContext.organizationId,
            authContext.username,
            authContext.email,
            authContext.fullName,
            authContext.role,
            authContext.permissions
        );
    }
}
