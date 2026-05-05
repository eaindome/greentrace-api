using Cai;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.StageFields;

[Queries]
public partial class StageFieldQueries
{
    [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 50)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<StageField> stageFields(AppDbContext db, AuthContext authContext)
    {
        if (!authContext.isLoggedIn) return Array.Empty<StageField>().AsQueryable();

        var query = db.stageFields.AsNoTracking();

        // Org-scope via stage → domain → organizationId
        if (!authContext.isPlatformAdmin)
            query = query.Where(sf => sf.stage!.domain!.organizationId == authContext.organizationId);

        return query;
    }
}
