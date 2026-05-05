using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Evidence;

[Queries]
public partial class EvidenceQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Evidence> evidences(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<Evidence>().AsQueryable();

        var query = db.evidences.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.ForOrganization(authContext.organizationId);

        return query;
    }
}
