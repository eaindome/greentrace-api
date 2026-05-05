using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Credentials;

[Queries]
public partial class CredentialQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Credential> credentials(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<Credential>().AsQueryable();

        var query = db.credentials.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.ForOrganization(authContext.organizationId);

        return query;
    }
}
