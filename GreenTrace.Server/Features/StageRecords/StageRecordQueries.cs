using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Extensions;
using GreenTrace.Server.Features.StageRecordFields;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.StageRecords;

[Queries]
public partial class StageRecordQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<StageRecord> stageRecords(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<StageRecord>().AsQueryable();

        var query = db.stageRecords.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.ForOrganization(authContext.organizationId);

        return query;
    }

    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 50)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<StageRecordField> stageRecordFields(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<StageRecordField>().AsQueryable();

        var query = db.stageRecordFields.AsNoTracking();

        if (!authContext.isPlatformAdmin)
            query = query.Where(srf => srf.stageRecord!.organizationId == authContext.organizationId);

        return query;
    }
}
