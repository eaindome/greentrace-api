using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Organizations;

[Queries]
public partial class OrganizationQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Organization> organizations(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<Organization>().AsQueryable();
        return db.organizations.AsNoTracking();
    }
}
