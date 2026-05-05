namespace GreenTrace.Server.Common.Data.Contracts;

/// <summary>
/// Marks an entity as belonging to an organization (tenant).
/// Used for automatic org-scoping in queries.
/// </summary>
public interface IHasOrganization
{
    long organizationId { get; set; }
}
