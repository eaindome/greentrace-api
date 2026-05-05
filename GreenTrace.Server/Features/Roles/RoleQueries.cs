using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Roles;

[Queries]
public partial class RoleQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> roles(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<Role>().AsQueryable();
        return db.Set<Role>().AsNoTracking();
    }

    /// <summary>
    /// Returns all available permission keys for the UI permission picker.
    /// </summary>
    public string[] availablePermissions() => Permissions.All;
}
