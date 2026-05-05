using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Roles;

public class RoleHandler
{
    public static async Task<CallResult> Handle(
        CreateRole input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.RolesCreate)) return CallResult.NotPermitted();

        var exists = await db.Set<Role>().AnyAsync(r => r.Name == input.name, cancellation);
        if (exists)
            return CallResult.Error("A role with this name already exists");

        var role = new Role
        {
            Name = input.name,
            NormalizedName = input.name.ToUpperInvariant(),
            description = input.description,
            permissions = input.permissions,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.Set<Role>().Add(role);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Role created");
    }

    public static async Task<CallResult> Handle(
        UpdateRole input,
        AppDbContext db,
        AuthContext authContext,
        UserService userService,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.RolesUpdate)) return CallResult.NotPermitted();

        var role = await db.Set<Role>().FindAsync([input.id], cancellation);
        if (role == null)
            return CallResult.Error("Role not found");

        if (input.description != null) role.description = input.description;
        if (input.permissions != null) role.permissions = input.permissions;
        if (input.isActive.HasValue) role.isActive = input.isActive.Value;
        role.updatedBy = authContext.email ?? "system";

        await db.SaveChangesAsync(cancellation);

        // Reload cache for all users with this role (permissions may have changed)
        if (input.permissions != null)
            await userService.LoadAllUsers();

        return CallResult.Ok("Role updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateRole input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.RolesDelete)) return CallResult.NotPermitted();

        var role = await db.Set<Role>().FindAsync([input.id], cancellation);
        if (role == null)
            return CallResult.Error("Role not found");

        role.isActive = false;
        role.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Role deactivated");
    }
}
