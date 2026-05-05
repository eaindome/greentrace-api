using System.ComponentModel.DataAnnotations;
using GreenTrace.Server.Common.Data.Contracts;

namespace GreenTrace.Server.Common.Data;

/// <summary>
/// Base record for all entities. Provides:
/// - UUID primary key
/// - Audit fields (createdBy, createdOn, updatedBy, updatedOn)
/// - Revision tracking
/// </summary>
public record Auditable : IAuditable
{
    public long id { get; set; }

    [Required]
    [StringLength(256)]
    public string createdBy { get; set; } = "";
    public DateTimeOffset createdOn { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    [StringLength(256)]
    public string updatedBy { get; set; } = "";
    public DateTimeOffset? updatedOn { get; set; } = DateTimeOffset.UtcNow;

    public int revision { get; set; }
}
