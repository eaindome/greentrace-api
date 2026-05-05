using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Auth;

public record AuthResponse(string accessToken, string refreshToken, DateTimeOffset expiresAt);

[Mutation<CallResult<AuthResponse>>]
public record Login(string email, string password);

[Mutation<CallResult<AuthResponse>>]
public record Register(string email, string password, string fullName, string? phone, long organizationId, string? roleName);

[Mutation<CallResult<AuthResponse>>]
public record RefreshAccessToken(string refreshToken);

[Mutation<CallResult>]
public record Logout(string refreshToken);
