using System.Reflection;

namespace GreenTrace.Server.Common.Auth;

/// <summary>
/// All permission keys in the system. Stored as string[] on Role entities.
/// Permissions.All is computed via reflection — adding a new const auto-includes it.
/// </summary>
public static class Permissions
{
    // Organizations
    public const string OrganizationsView = "organizations.view";
    public const string OrganizationsCreate = "organizations.create";
    public const string OrganizationsUpdate = "organizations.update";
    public const string OrganizationsDelete = "organizations.delete";

    // Users
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";

    // Roles
    public const string RolesView = "roles.view";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";

    // Domains
    public const string DomainsView = "domains.view";
    public const string DomainsCreate = "domains.create";
    public const string DomainsUpdate = "domains.update";
    public const string DomainsDelete = "domains.delete";

    // Stages
    public const string StagesView = "stages.view";
    public const string StagesCreate = "stages.create";
    public const string StagesUpdate = "stages.update";
    public const string StagesDelete = "stages.delete";

    // Products
    public const string ProductsView = "products.view";
    public const string ProductsCreate = "products.create";
    public const string ProductsUpdate = "products.update";
    public const string ProductsDelete = "products.delete";

    // Batches
    public const string BatchesView = "batches.view";
    public const string BatchesCreate = "batches.create";
    public const string BatchesUpdate = "batches.update";
    public const string BatchesDelete = "batches.delete";

    // Stage Records
    public const string StageRecordsView = "stage_records.view";
    public const string StageRecordsCreate = "stage_records.create";
    public const string StageRecordsUpdate = "stage_records.update";
    public const string StageRecordsValidate = "stage_records.validate";

    // Analytics
    public const string AnalyticsView = "analytics.view";

    // Audit
    public const string AuditView = "audit.view";

    // Integrations
    public const string IntegrationsView = "integrations.view";
    public const string IntegrationsCreate = "integrations.create";
    public const string IntegrationsUpdate = "integrations.update";
    public const string IntegrationsDelete = "integrations.delete";

    /// <summary>
    /// All permission keys, auto-collected via reflection.
    /// </summary>
    public static readonly string[] All = typeof(Permissions)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.FieldType == typeof(string))
        .Select(f => (string)f.GetRawConstantValue()!)
        .ToArray();
}
