using Cai;
using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Analytics;

[Queries]
public partial class AnalyticsQueries
{
    public async Task<List<BatchStatusSummary>> batchStatusSummary(
        AppDbContext db, AuthContext authContext, CancellationToken ct)
    {
        if (!authContext.isLoggedIn) return [];

        var query = db.batches.AsNoTracking().Where(b => !b.isDeleted);
        if (!authContext.isPlatformAdmin)
            query = query.Where(b => b.organizationId == authContext.organizationId);

        return await query
            .GroupBy(b => b.status)
            .Select(g => new BatchStatusSummary(g.Key, g.Count()))
            .ToListAsync(ct);
    }

    public async Task<List<ValueByDomain>> valueByDomain(
        AppDbContext db, AuthContext authContext, CancellationToken ct)
    {
        if (!authContext.isLoggedIn) return [];

        var query = db.valueRecords.AsNoTracking();
        if (!authContext.isPlatformAdmin)
            query = query.Where(v => v.organizationId == authContext.organizationId);

        return await query
            .Join(db.batches, v => v.batchId, b => b.id, (v, b) => new { v, b })
            .Join(db.domains, vb => vb.b.domainId, d => d.id, (vb, d) => new { vb.v, d })
            .GroupBy(x => new { x.d.id, x.d.name, x.v.currency })
            .Select(g => new ValueByDomain(g.Key.id, g.Key.name, g.Sum(x => x.v.totalValue), g.Key.currency))
            .ToListAsync(ct);
    }

    public async Task<List<ActorValueRanking>> topActorsByValue(
        int topN, AppDbContext db, AuthContext authContext, CancellationToken ct)
    {
        if (!authContext.isLoggedIn) return [];
        if (topN <= 0) topN = 10;

        var query = db.valueRecords.AsNoTracking().Where(v => v.actorId != null);
        if (!authContext.isPlatformAdmin)
            query = query.Where(v => v.organizationId == authContext.organizationId);

        return await query
            .Join(db.actors, v => v.actorId, a => a.id, (v, a) => new { v, a })
            .GroupBy(x => new { x.a.id, x.a.name, x.v.currency })
            .Select(g => new ActorValueRanking(
                g.Key.id, g.Key.name, g.Sum(x => x.v.totalValue), g.Key.currency, g.Count()))
            .OrderByDescending(r => r.totalValue)
            .Take(topN)
            .ToListAsync(ct);
    }

    public async Task<List<StageCompletionRate>> stageCompletionRates(
        long domainId, AppDbContext db, AuthContext authContext, CancellationToken ct)
    {
        if (!authContext.isLoggedIn) return [];

        var stagesQuery = db.stages.AsNoTracking().Where(s => s.domainId == domainId && s.isActive);

        // Total batches in this domain
        var batchesQuery = db.batches.AsNoTracking().Where(b => b.domainId == domainId && !b.isDeleted);
        if (!authContext.isPlatformAdmin)
            batchesQuery = batchesQuery.Where(b => b.organizationId == authContext.organizationId);
        var totalBatches = await batchesQuery.CountAsync(ct);
        if (totalBatches == 0) return [];

        var validatedRecords = db.stageRecords.AsNoTracking()
            .Where(sr => sr.status == Constants.RecordValidated && !sr.isDeleted);
        if (!authContext.isPlatformAdmin)
            validatedRecords = validatedRecords.Where(sr => sr.organizationId == authContext.organizationId);

        var stages = await stagesQuery
            .OrderBy(s => s.sequence)
            .Select(s => new
            {
                s.id,
                s.name,
                s.sequence,
                completedBatches = validatedRecords
                    .Where(sr => sr.stageId == s.id)
                    .Select(sr => sr.batchId)
                    .Distinct()
                    .Count()
            })
            .ToListAsync(ct);

        return stages
            .Select(s => new StageCompletionRate(
                s.id, s.name, s.sequence, s.completedBatches,
                Math.Round((double)s.completedBatches / totalBatches * 100, 2)))
            .ToList();
    }

    public async Task<List<ValueOverTime>> valueOverTime(
        int? year, AppDbContext db, AuthContext authContext, CancellationToken ct)
    {
        if (!authContext.isLoggedIn) return [];

        var query = db.valueRecords.AsNoTracking();
        if (!authContext.isPlatformAdmin)
            query = query.Where(v => v.organizationId == authContext.organizationId);

        if (year.HasValue)
            query = query.Where(v => v.validatedAt != null && v.validatedAt.Value.Year == year.Value);

        return await query
            .Where(v => v.validatedAt != null)
            .GroupBy(v => new { v.validatedAt!.Value.Year, v.validatedAt!.Value.Month })
            .Select(g => new ValueOverTime(g.Key.Year, g.Key.Month, g.Sum(v => v.totalValue), g.Count()))
            .OrderBy(v => v.year).ThenBy(v => v.month)
            .ToListAsync(ct);
    }
}
