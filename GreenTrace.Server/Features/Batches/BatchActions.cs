using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Batches;

[Mutation<CallResult>]
public record CreateBatch(
    long organizationId, long domainId, long? productId, string batchCode,
    long? originActorId, DateTimeOffset? originDate, string? originLocation,
    decimal? initialQuantity, decimal? initialUnitPrice, string? metadata);

[Mutation<CallResult>]
public record UpdateBatch(
    long id, long? productId, string? originLocation,
    decimal? currentQuantity, string? status, string? metadata);

[Mutation<CallResult>]
public record DeleteBatch(long id);

[Mutation<CallResult>]
public record SplitBatch(long parentBatchId, string childBatchCode, decimal quantity, string? notes);

[Mutation<CallResult>]
public record MergeBatches(long[] parentBatchIds, string childBatchCode, string? notes);
