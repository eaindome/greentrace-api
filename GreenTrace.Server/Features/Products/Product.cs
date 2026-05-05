using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Features.Products;

/// <summary>
/// What is being tracked through the supply chain (e.g. "Cocoa Beans", "Raw Gold").
/// </summary>
public record Product : Auditable, IHasOrganization
{
    public long organizationId { get; set; }
    public long? domainId { get; set; }
    public string name { get; set; } = default!;
    public string? sku { get; set; }               // Org's product code
    public string? unitOfMeasure { get; set; }      // e.g. "kg", "oz", "tonnes"
    public string? description { get; set; }
    public bool isActive { get; set; } = true;

    // Navigation
    public Organizations.Organization? organization { get; set; }
    public Domains.Domain? domain { get; set; }
    public ICollection<Batches.Batch>? batches { get; set; }
}
