using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Users;

public class UserHandler
{
    public static async Task<CallResult> Handle(
        CreateUser input,
        UserManager<User> userManager,
        AppDbContext db,
        AuthContext authContext,
        UserService userService,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.UsersCreate)) return CallResult.NotPermitted();

        var existingUser = await userManager.FindByEmailAsync(input.email);
        if (existingUser != null)
            return CallResult.Error("A user with this email already exists");

        var orgExists = await db.organizations.AnyAsync(o => o.id == input.organizationId, cancellation);
        if (!orgExists)
            return CallResult.Error("Organization not found");

        // Non-platform-admins can only create users in their own org
        if (!authContext.isPlatformAdmin && input.organizationId != authContext.organizationId)
            return CallResult.Error("You can only create users in your own organization");

        var roleExists = await db.Set<Role>().AnyAsync(r => r.Name == input.roleName, cancellation);
        if (!roleExists)
            return CallResult.Error($"Role '{input.roleName}' does not exist");

        var user = new User
        {
            Email = input.email,
            UserName = input.email,
            fullName = input.fullName,
            PhoneNumber = input.phone,
            organizationId = input.organizationId,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        var result = await userManager.CreateAsync(user, input.password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return CallResult.Error($"Failed to create user: {errors}");
        }

        await userManager.AddToRoleAsync(user, input.roleName);

        var role = await db.Set<Role>().AsNoTracking().FirstOrDefaultAsync(r => r.Name == input.roleName, cancellation);
        userService.Set(new CachedUser(
            user.Id, user.organizationId, user.fullName,
            user.Email ?? "", user.UserName, input.roleName,
            role?.permissions ?? [], user.isActive
        ));

        return CallResult.Ok("User created successfully");
    }

    public static async Task<CallResult> Handle(
        UpdateUser input,
        UserManager<User> userManager,
        AppDbContext db,
        AuthContext authContext,
        UserService userService,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.UsersUpdate)) return CallResult.NotPermitted();

        var user = await userManager.FindByIdAsync(input.id.ToString());
        if (user == null)
            return CallResult.Error("User not found");

        // Non-platform-admins can only update users in their own org
        if (!authContext.isPlatformAdmin && user.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.fullName != null) user.fullName = input.fullName;
        if (input.phone != null) user.PhoneNumber = input.phone;
        if (input.isActive.HasValue) user.isActive = input.isActive.Value;
        user.updatedBy = authContext.email ?? "system";

        await userManager.UpdateAsync(user);

        // Handle role change
        var roleName = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
        if (input.roleName != null && input.roleName != roleName)
        {
            var newRoleExists = await db.Set<Role>().AnyAsync(r => r.Name == input.roleName, cancellation);
            if (!newRoleExists)
                return CallResult.Error($"Role '{input.roleName}' does not exist");

            if (!string.IsNullOrEmpty(roleName))
                await userManager.RemoveFromRoleAsync(user, roleName);
            await userManager.AddToRoleAsync(user, input.roleName);
            roleName = input.roleName;
        }

        var role = await db.Set<Role>().AsNoTracking().FirstOrDefaultAsync(r => r.Name == roleName, cancellation);
        userService.Set(new CachedUser(
            user.Id, user.organizationId, user.fullName,
            user.Email ?? "", user.UserName, roleName,
            role?.permissions ?? [], user.isActive
        ));

        return CallResult.Ok("User updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateUser input,
        UserManager<User> userManager,
        AuthContext authContext,
        UserService userService,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.UsersDelete)) return CallResult.NotPermitted();

        var user = await userManager.FindByIdAsync(input.id.ToString());
        if (user == null)
            return CallResult.Error("User not found");

        if (!authContext.isPlatformAdmin && user.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        user.isActive = false;
        user.updatedBy = authContext.email ?? "system";
        await userManager.UpdateAsync(user);

        userService.Invalidate(user.Id);

        return CallResult.Ok("User deactivated");
    }
}
