using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Audit;

[Queries]
public partial class AuditQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<AuditLog> auditLogs(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<AuditLog>().AsQueryable();

        var query = db.auditLogs.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.Where(a => a.organizationId == authContext.organizationId);

        return query;
    }
}
