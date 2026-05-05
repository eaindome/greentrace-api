using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Stages;

[Queries]
public partial class StageQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Stage> stages(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<Stage>().AsQueryable();

        var query = db.stages.AsNoTracking();

        // Org-scope via domain's organizationId
        if (!authContext.isPlatformAdmin)
            query = query.Where(s => s.domain!.organizationId == authContext.organizationId);

        return query;
    }
}
