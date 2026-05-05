using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Roles;
using GreenTrace.Server.Features.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Auth;

public class AuthHandler
{
    public static async Task<CallResult<AuthResponse>> Handle(
        Login input,
        UserManager<User> userManager,
        AppDbContext db,
        TokenService tokenService,
        UserService userService,
        CancellationToken cancellation)
    {
        var user = await userManager.FindByEmailAsync(input.email);
        if (user == null || !user.isActive)
            return CallResult<AuthResponse>.Error("Invalid email or password");

        var validPassword = await userManager.CheckPasswordAsync(user, input.password);
        if (!validPassword)
            return CallResult<AuthResponse>.Error("Invalid email or password");

        var roles = await userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault() ?? "";

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, roleName, user.organizationId);
        var refreshTokenValue = tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            userId = user.Id,
            token = refreshTokenValue,
            expiresAt = tokenService.GetRefreshTokenExpiry(),
            createdBy = user.Email ?? "system",
            updatedBy = user.Email ?? "system"
        };

        db.refreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellation);

        // Ensure user is in cache
        var role = await db.Set<Role>().AsNoTracking().FirstOrDefaultAsync(r => r.Name == roleName, cancellation);
        userService.Set(new CachedUser(
            user.Id, user.organizationId, user.fullName,
            user.Email ?? "", user.UserName, roleName,
            role?.permissions ?? [], user.isActive
        ));

        return CallResult<AuthResponse>.Ok(
            new AuthResponse(accessToken, refreshTokenValue, expiresAt),
            "Login successful"
        );
    }

    public static async Task<CallResult<AuthResponse>> Handle(
        Register input,
        UserManager<User> userManager,
        AppDbContext db,
        TokenService tokenService,
        UserService userService,
        CancellationToken cancellation)
    {
        var existingUser = await userManager.FindByEmailAsync(input.email);
        if (existingUser != null)
            return CallResult<AuthResponse>.Error("A user with this email already exists");

        var orgExists = await db.organizations.AnyAsync(o => o.id == input.organizationId, cancellation);
        if (!orgExists)
            return CallResult<AuthResponse>.Error("Organization not found");

        var roleName = input.roleName ?? Constants.FieldAgent;
        var roleExists = await db.Set<Role>().AnyAsync(r => r.Name == roleName, cancellation);
        if (!roleExists)
            return CallResult<AuthResponse>.Error($"Role '{roleName}' does not exist");

        var user = new User
        {
            Email = input.email,
            UserName = input.email,
            fullName = input.fullName,
            PhoneNumber = input.phone,
            organizationId = input.organizationId,
            createdBy = input.email,
            updatedBy = input.email
        };

        var result = await userManager.CreateAsync(user, input.password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return CallResult<AuthResponse>.Error($"Registration failed: {errors}");
        }

        await userManager.AddToRoleAsync(user, roleName);

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, roleName, user.organizationId);
        var refreshTokenValue = tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            userId = user.Id,
            token = refreshTokenValue,
            expiresAt = tokenService.GetRefreshTokenExpiry(),
            createdBy = input.email,
            updatedBy = input.email
        };

        db.refreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellation);

        var role = await db.Set<Role>().AsNoTracking().FirstOrDefaultAsync(r => r.Name == roleName, cancellation);
        userService.Set(new CachedUser(
            user.Id, user.organizationId, user.fullName,
            user.Email ?? "", user.UserName, roleName,
            role?.permissions ?? [], user.isActive
        ));

        return CallResult<AuthResponse>.Ok(
            new AuthResponse(accessToken, refreshTokenValue, expiresAt),
            "Registration successful"
        );
    }

    public static async Task<CallResult<AuthResponse>> Handle(
        RefreshAccessToken input,
        AppDbContext db,
        UserManager<User> userManager,
        TokenService tokenService,
        UserService userService,
        CancellationToken cancellation)
    {
        var storedToken = await db.refreshTokens
            .FirstOrDefaultAsync(rt => rt.token == input.refreshToken && !rt.isRevoked, cancellation);

        if (storedToken == null)
            return CallResult<AuthResponse>.Error("Invalid refresh token");

        if (storedToken.expiresAt < DateTimeOffset.UtcNow)
        {
            storedToken.isRevoked = true;
            await db.SaveChangesAsync(cancellation);
            return CallResult<AuthResponse>.Error("Refresh token expired");
        }

        var user = await userManager.FindByIdAsync(storedToken.userId.ToString());
        if (user == null || !user.isActive)
            return CallResult<AuthResponse>.Error("User not found or inactive");

        // Revoke old token
        storedToken.isRevoked = true;

        var roles = await userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault() ?? "";

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, roleName, user.organizationId);
        var newRefreshTokenValue = tokenService.GenerateRefreshToken();

        storedToken.replacedByToken = newRefreshTokenValue;

        var newRefreshToken = new RefreshToken
        {
            userId = user.Id,
            token = newRefreshTokenValue,
            expiresAt = tokenService.GetRefreshTokenExpiry(),
            createdBy = user.Email ?? "system",
            updatedBy = user.Email ?? "system"
        };

        db.refreshTokens.Add(newRefreshToken);
        await db.SaveChangesAsync(cancellation);

        return CallResult<AuthResponse>.Ok(
            new AuthResponse(accessToken, newRefreshTokenValue, expiresAt),
            "Token refreshed"
        );
    }

    public static async Task<CallResult> Handle(
        Logout input,
        AppDbContext db,
        CancellationToken cancellation)
    {
        var token = await db.refreshTokens
            .FirstOrDefaultAsync(rt => rt.token == input.refreshToken && !rt.isRevoked, cancellation);

        if (token != null)
        {
            token.isRevoked = true;
            await db.SaveChangesAsync(cancellation);
        }

        return CallResult.Ok("Logged out");
    }
}
