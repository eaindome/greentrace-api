using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GreenTrace.Server.Features.Users;
using Microsoft.IdentityModel.Tokens;

namespace GreenTrace.Server.Common.Auth;

/// <summary>
/// JWT settings bound from appsettings.json "Jwt" section.
/// </summary>
public record JwtSettings
{
    public string Secret { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int AccessTokenExpirationMinutes { get; init; } = 60;
    public int RefreshTokenExpirationDays { get; init; } = 7;
}

/// <summary>
/// Generates JWT access tokens and cryptographic refresh tokens.
/// Permissions are NOT included in the JWT to keep it small — frontend fetches via `me` query.
/// </summary>
public class TokenService
{
    private readonly JwtSettings _settings;

    public TokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    public (string token, DateTimeOffset expiresAt) GenerateAccessToken(User user, string roleName, long organizationId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, roleName),
            new("organizationId", organizationId.ToString()),
            new("fullName", user.fullName),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public DateTimeOffset GetRefreshTokenExpiry()
    {
        return DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
    }
}
