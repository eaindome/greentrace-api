using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GreenTrace.Server.Common.Auth;

/// <summary>
/// Per-request authentication context. Parses userId from JWT,
/// fetches the rest from UserService cache.
/// </summary>
public class AuthContext
{
    public bool isLoggedIn { get; init; }
    public long userId { get; init; }
    public long organizationId { get; init; }
    public string? username { get; init; }
    public string? email { get; init; }
    public string? fullName { get; init; }
    public string? role { get; init; }
    public string[] permissions { get; init; } = [];

    public AuthContext(IHttpContextAccessor http, UserService userService)
    {
        if (http?.HttpContext?.User?.Identity?.IsAuthenticated != true)
        {
            isLoggedIn = false;
            return;
        }

        var principal = http.HttpContext.User;
        isLoggedIn = true;

        if (!long.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier), out var uid))
            return;

        userId = uid;

        // Fetch enriched data from cache
        var cached = userService.GetUser(uid);
        if (cached != null)
        {
            organizationId = cached.organizationId;
            username = cached.username;
            email = cached.email;
            fullName = cached.fullName;
            role = cached.roleName;
            permissions = cached.permissions;
        }
        else
        {
            // Fallback to JWT claims if cache miss
            if (long.TryParse(principal.FindFirstValue("organizationId"), out var orgId))
                organizationId = orgId;

            username = principal.FindFirstValue(ClaimTypes.Name);
            email = principal.FindFirstValue(ClaimTypes.Email);
            fullName = principal.FindFirstValue("fullName");
            role = principal.FindFirstValue(ClaimTypes.Role);
        }
    }

    // Convenience checks
    public bool isPlatformAdmin => role == Constants.PlatformAdmin;
    public bool isAnyAdmin => role == Constants.OrgAdmin || role == Constants.PlatformAdmin;

    /// <summary>
    /// Returns true if platformAdmin OR permissions array contains the key.
    /// </summary>
    public bool hasPermission(string permission) =>
        isPlatformAdmin || permissions.Contains(permission);
}
