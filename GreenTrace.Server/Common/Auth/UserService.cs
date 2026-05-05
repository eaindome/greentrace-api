using System.Collections.Concurrent;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Roles;
using GreenTrace.Server.Features.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Auth;

/// <summary>
/// Cached user record. Avoids DB hits on every request.
/// </summary>
public record CachedUser(
    long userId,
    long organizationId,
    string fullName,
    string email,
    string? username,
    string roleName,
    string[] permissions,
    bool isActive
);

/// <summary>
/// Background service that caches user + role data in memory.
/// Loads all users at startup, then handlers call Set/Invalidate after mutations.
/// </summary>
public class UserService : BackgroundService
{
    private readonly ConcurrentDictionary<long, CachedUser> _cache = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserService> _logger;

    public UserService(IServiceScopeFactory scopeFactory, ILogger<UserService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public CachedUser? GetUser(long userId)
    {
        _cache.TryGetValue(userId, out var user);
        return user;
    }

    public void Set(CachedUser user)
    {
        _cache[user.userId] = user;
    }

    public void Invalidate(long userId)
    {
        _cache.TryRemove(userId, out _);
    }

    public async Task LoadAllUsers()
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var users = await db.Users.AsNoTracking().ToListAsync();
        var roles = await db.Set<Role>().AsNoTracking().ToDictionaryAsync(r => r.Id);

        foreach (var user in users)
        {
            var roleNames = await userManager.GetRolesAsync(user);
            var roleName = roleNames.FirstOrDefault() ?? "";

            string[] permissions = [];
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = roles.Values.FirstOrDefault(r => r.Name == roleName);
                permissions = role?.permissions ?? [];
            }

            _cache[user.Id] = new CachedUser(
                user.Id,
                user.organizationId,
                user.fullName,
                user.Email ?? "",
                user.UserName,
                roleName,
                permissions,
                user.isActive
            );
        }

        _logger.LogInformation("UserService cache loaded with {Count} users", _cache.Count);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await LoadAllUsers();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user cache on startup");
        }
    }
}
