using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Common.Extensions;

/// <summary>
/// Query extension methods for org-scoping and common filters.
/// Use these to ensure all queries are always scoped to the current organization.
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    /// Filters a queryable to only include entities belonging to the specified organization.
    /// This is the primary tenant isolation mechanism.
    /// </summary>
    public static IQueryable<T> ForOrganization<T>(this IQueryable<T> query, long organizationId)
        where T : IHasOrganization
    {
        return query.Where(e => e.organizationId == organizationId);
    }
}
