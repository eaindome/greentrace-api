using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.ActorRoles;

[Queries]
public partial class ActorRoleQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ActorRole> actorRoles(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<ActorRole>().AsQueryable();

        var query = db.actorRoles.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.ForOrganization(authContext.organizationId);

        return query;
    }
}
