namespace GreenTrace.Server.Common;

/// <summary>
/// Application-wide constants. Enums and known values go here.
/// </summary>
public static class Constants
{
    // ── User Roles (platform users) ──────────────────────────
    public const string PlatformAdmin = "platform_admin";
    public const string OrgAdmin = "org_admin";
    public const string ChainManager = "chain_manager";
    public const string FieldAgent = "field_agent";
    public const string Validator = "validator";
    public const string Analyst = "analyst";

    // ── Batch Status ─────────────────────────────────────────
    public const string BatchOpen = "open";
    public const string BatchInProgress = "in_progress";
    public const string BatchCompleted = "completed";
    public const string BatchArchived = "archived";

    // ── Stage Record Status ──────────────────────────────────
    public const string RecordDraft = "draft";
    public const string RecordSubmitted = "submitted";
    public const string RecordValidated = "validated";
    public const string RecordRejected = "rejected";

    // ── Stage Field Data Types ───────────────────────────────
    public const string FieldTypeText = "text";
    public const string FieldTypeNumber = "number";
    public const string FieldTypeDecimal = "decimal";
    public const string FieldTypeBoolean = "boolean";
    public const string FieldTypeDate = "date";
    public const string FieldTypeSelect = "select";
    public const string FieldTypeFile = "file";
    public const string FieldTypeGeoPoint = "geo_point";

    // ── Audit Actions ──────────────────────────────────────
    public const string AuditCreated = "created";
    public const string AuditUpdated = "updated";
    public const string AuditDeleted = "deleted";

    // ── Integration Types ────────────────────────────────────
    public const string IntegrationUssd = "ussd";
    public const string IntegrationSms = "sms";
    public const string IntegrationWhatsApp = "whatsapp";
    public const string IntegrationWebhook = "http_webhook";
    public const string IntegrationSftp = "sftp";
}
