using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.ValueChain;

[Queries]
public partial class ValueChainQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ValueRecord> valueRecords(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<ValueRecord>().AsQueryable();

        var query = db.valueRecords.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.ForOrganization(authContext.organizationId);

        return query;
    }
}
