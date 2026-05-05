using GreenTrace.Server.Features.Roles;
using GreenTrace.Server.Features.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

/// <summary>
/// Main database context. Extends IdentityDbContext for ASP.NET Identity tables.
/// Each feature adds its own DbSets via partial classes.
/// See: Features/*/DataContext.cs
/// </summary>
public partial class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, Role, long>(options)
{
    public async Task<int> Save<T>(T entity, CancellationToken cancellationToken = default)
    {
        await AddAsync(entity!, cancellationToken: cancellationToken);
        return await SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Critical — configures Identity tables (AspNetUsers, AspNetRoles, etc.)
        base.OnModelCreating(modelBuilder);

        // Feature configurations
        ConfigureOrganizations(modelBuilder);
        ConfigureUsers(modelBuilder);
        ConfigureRoles(modelBuilder);
        ConfigureAuth(modelBuilder);
        ConfigureDomains(modelBuilder);
        ConfigureStages(modelBuilder);
        ConfigureStageFields(modelBuilder);
        ConfigureActorRoles(modelBuilder);
        ConfigureActors(modelBuilder);
        ConfigureCredentials(modelBuilder);
        ConfigureProducts(modelBuilder);
        ConfigureBatches(modelBuilder);
        ConfigureStageRecords(modelBuilder);
        ConfigureStageRecordFields(modelBuilder);
        ConfigureEvidence(modelBuilder);
        ConfigureValueChain(modelBuilder);
        ConfigureAudit(modelBuilder);
    }
}
