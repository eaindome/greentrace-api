using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data.Contracts;
using GreenTrace.Server.Features.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GreenTrace.Server.Common.Data;

/// <summary>
/// EF Core interceptor that automatically sets audit fields (createdBy, updatedBy, etc.)
/// on SaveChanges. When constructed with DI services, also writes AuditLog entries.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor? _http;
    private readonly UserService? _userService;

    /// <summary>Parameterless — timestamps only (used by pooled factory).</summary>
    public AuditInterceptor() { }

    /// <summary>DI-aware — timestamps + audit log writes (used by scoped DbContext).</summary>
    public AuditInterceptor(IHttpContextAccessor http, UserService userService)
    {
        _http = http;
        _userService = userService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditFields(eventData.Context);
        AppendAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditFields(eventData.Context);
        AppendAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyAuditFields(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.createdOn = DateTimeOffset.UtcNow;
                entry.Entity.updatedOn = DateTimeOffset.UtcNow;
                entry.Entity.revision = 1;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.updatedOn = DateTimeOffset.UtcNow;
                entry.Entity.revision++;

                // Prevent overwriting createdBy and createdOn on updates
                entry.Property(e => e.createdBy).IsModified = false;
                entry.Property(e => e.createdOn).IsModified = false;
            }
        }
    }

    private void AppendAuditLogs(DbContext? context)
    {
        if (context == null || _http == null || _userService == null) return;

        // Extract user info from JWT claims
        var httpContext = _http.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true) return;

        long? userId = null;
        string userEmail = "";
        long? organizationId = null;

        var sub = httpContext.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
            ?? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (sub != null && long.TryParse(sub.Value, out var uid))
        {
            userId = uid;
            var cached = _userService.GetUser(uid);
            if (cached != null)
            {
                userEmail = cached.email;
                organizationId = cached.organizationId;
            }
        }

        var logs = AuditService.BuildChangeLogs(context, userId, userEmail, organizationId);
        foreach (var log in logs)
            context.Set<AuditLog>().Add(log);
    }
}
