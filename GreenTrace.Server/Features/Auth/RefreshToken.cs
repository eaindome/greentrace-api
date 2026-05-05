using GreenTrace.Server.Common.Data;

namespace GreenTrace.Server.Features.Auth;

/// <summary>
/// Stored refresh token for JWT token rotation.
/// </summary>
public record RefreshToken : Auditable
{
    public long userId { get; set; }
    public string token { get; set; } = default!;
    public DateTimeOffset expiresAt { get; set; }
    public bool isRevoked { get; set; }
    public string? replacedByToken { get; set; }
}
