namespace GreenTrace.Server.Common.Data.Contracts;

public interface IAuditable : IHasId
{
    string createdBy { get; set; }
    DateTimeOffset createdOn { get; set; }
    string updatedBy { get; set; }
    DateTimeOffset? updatedOn { get; set; }
    int revision { get; set; }
}
