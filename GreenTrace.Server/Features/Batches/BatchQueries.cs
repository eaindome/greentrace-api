using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Batches;

[Queries]
public partial class BatchQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Batch> batches(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<Batch>().AsQueryable();

        var query = db.batches.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.ForOrganization(authContext.organizationId);

        return query;
    }

    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<BatchLineage> batchLineages(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<BatchLineage>().AsQueryable();

        var query = db.batchLineages.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.ForOrganization(authContext.organizationId);

        return query;
    }
}
