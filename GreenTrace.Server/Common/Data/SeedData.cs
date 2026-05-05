using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Features.Organizations;
using GreenTrace.Server.Features.Roles;
using GreenTrace.Server.Features.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<AppDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

        await db.Database.MigrateAsync();

        await SeedRoles(roleManager, logger);
        var org = await SeedOrganization(db, logger);
        await SeedAdminUser(userManager, org.id, logger);
    }

    private static async Task SeedRoles(RoleManager<Role> roleManager, ILogger logger)
    {
        var roles = new (string name, string description, string[] permissions)[]
        {
            (Constants.PlatformAdmin, "Full system access", Permissions.All),
            (Constants.OrgAdmin, "Organization administrator", new[]
            {
                Permissions.OrganizationsView, Permissions.OrganizationsUpdate,
                Permissions.UsersView, Permissions.UsersCreate, Permissions.UsersUpdate, Permissions.UsersDelete,
                Permissions.RolesView,
                Permissions.DomainsView, Permissions.DomainsCreate, Permissions.DomainsUpdate, Permissions.DomainsDelete,
                Permissions.StagesView, Permissions.StagesCreate, Permissions.StagesUpdate, Permissions.StagesDelete,
                Permissions.ProductsView, Permissions.ProductsCreate, Permissions.ProductsUpdate, Permissions.ProductsDelete,
                Permissions.BatchesView, Permissions.BatchesCreate, Permissions.BatchesUpdate, Permissions.BatchesDelete,
                Permissions.StageRecordsView, Permissions.StageRecordsCreate, Permissions.StageRecordsUpdate, Permissions.StageRecordsValidate,
                Permissions.AnalyticsView, Permissions.AuditView,
                Permissions.IntegrationsView, Permissions.IntegrationsCreate, Permissions.IntegrationsUpdate, Permissions.IntegrationsDelete,
            }),
            (Constants.ChainManager, "Manages supply chain domains and stages", new[]
            {
                Permissions.OrganizationsView,
                Permissions.DomainsView, Permissions.DomainsCreate, Permissions.DomainsUpdate,
                Permissions.StagesView, Permissions.StagesCreate, Permissions.StagesUpdate,
                Permissions.ProductsView, Permissions.ProductsCreate, Permissions.ProductsUpdate,
                Permissions.BatchesView, Permissions.BatchesCreate, Permissions.BatchesUpdate,
                Permissions.StageRecordsView,
                Permissions.AnalyticsView,
            }),
            (Constants.FieldAgent, "Records stage data in the field", new[]
            {
                Permissions.OrganizationsView,
                Permissions.DomainsView, Permissions.StagesView,
                Permissions.ProductsView, Permissions.BatchesView,
                Permissions.StageRecordsView, Permissions.StageRecordsCreate, Permissions.StageRecordsUpdate,
            }),
            (Constants.Validator, "Validates submitted stage records", new[]
            {
                Permissions.OrganizationsView,
                Permissions.DomainsView, Permissions.StagesView,
                Permissions.ProductsView, Permissions.BatchesView,
                Permissions.StageRecordsView, Permissions.StageRecordsValidate,
                Permissions.AuditView,
            }),
            (Constants.Analyst, "Read-only analytics and audit access", new[]
            {
                Permissions.OrganizationsView,
                Permissions.DomainsView, Permissions.StagesView,
                Permissions.ProductsView, Permissions.BatchesView,
                Permissions.StageRecordsView,
                Permissions.AnalyticsView, Permissions.AuditView,
            }),
        };

        foreach (var (name, description, permissions) in roles)
        {
            if (await roleManager.RoleExistsAsync(name)) continue;

            var role = new Role
            {
                Name = name,
                description = description,
                permissions = permissions,
                createdBy = "system",
                updatedBy = "system"
            };

            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
                logger.LogInformation("Seeded role: {Role}", name);
            else
                logger.LogWarning("Failed to seed role {Role}: {Errors}", name,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task<Organization> SeedOrganization(AppDbContext db, ILogger logger)
    {
        var org = await db.organizations.FirstOrDefaultAsync(o => o.slug == "greentrace");
        if (org != null) return org;

        org = new Organization
        {
            name = "GreenTrace",
            slug = "greentrace",
            createdBy = "system",
            updatedBy = "system"
        };

        db.organizations.Add(org);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded default organization: GreenTrace");

        return org;
    }

    private static async Task SeedAdminUser(UserManager<User> userManager, long orgId, ILogger logger)
    {
        const string adminEmail = "admin@greentrace.io";

        if (await userManager.FindByEmailAsync(adminEmail) != null) return;

        var admin = new User
        {
            Email = adminEmail,
            UserName = adminEmail,
            fullName = "Platform Admin",
            organizationId = orgId,
            EmailConfirmed = true,
            createdBy = "system",
            updatedBy = "system"
        };

        var result = await userManager.CreateAsync(admin, "Admin@123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, Constants.PlatformAdmin);
            logger.LogInformation("Seeded admin user: {Email}", adminEmail);
        }
        else
        {
            logger.LogWarning("Failed to seed admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
